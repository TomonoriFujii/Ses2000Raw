using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using FeatureType = DotSpatial.Data.FeatureType;
using PointShape = DotSpatial.Symbology.PointShape;
//using Coordinate = DotSpatial.Topology.Coordinate;
using Coordinate = NetTopologySuite.Geometries.Coordinate;
//using LineString = DotSpatial.Topology.LineString;
using LineString = NetTopologySuite.Geometries.LineString;

namespace Ses2000Raw
{
    public partial class MapForm : DockContent
    {
        private FeatureSet? m_trackFeatureSet;
        private IMapLineLayer? m_trackLayer;
        private FeatureSet? m_cursorFeatureSet;
        private IMapPointLayer? m_cursorLayer;
        private (double X, double Y)? m_currentCursor;

        public MapForm()
        {
            InitializeComponent(); map1.ProjectionModeDefine = ActionMode.Never;
            map1.ProjectionModeReproject = ActionMode.Never;

            //map1.BackColor = Color.Black;
            map1.BackColor = Constant.COMBO_BACKCOLOR;
        }

        /// <summary>
        /// Ping位置の航跡を線で表示する。
        /// </summary>
        /// <param name="coordinates">描画するXY座標（m単位想定）</param>
        public void SetTrack(IEnumerable<(double X, double Y)> coordinates)
        {
            var points = coordinates?.ToList() ?? new List<(double X, double Y)>();

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetTrack(points)));
                return;
            }

            //ClearTrackInternal();

            if (points.Count == 0)
            {
                map1.Refresh();
                return;
            }

            m_trackFeatureSet = new FeatureSet(FeatureType.Line);
            var coords = new List<Coordinate>(points.Count);
            foreach (var (x, y) in points)
            {
                coords.Add(new Coordinate(x, y));
            }

            if (coords.Count == 1)
            {
                // 1点だけの場合でも描画できるように重複点を追加
                coords.Add(new Coordinate(coords[0].X, coords[0].Y));
            }

            m_trackFeatureSet.AddFeature(new LineString(coords.ToArray()));

            m_trackLayer = map1.Layers.Add(m_trackFeatureSet) as IMapLineLayer;
            if (m_trackLayer != null)
            {
                m_trackLayer.Symbolizer = new LineSymbolizer(Color.WhiteSmoke, 1f);
                m_trackLayer.LegendText = "Track";
            }

            map1.ZoomToMaxExtent();

            map1.Refresh();

            if (m_currentCursor.HasValue)
            {
                // 軌跡の再描画後にカーソルを最前面に戻す
                var (x, y) = m_currentCursor.Value;
                UpdateCursorPosition(x, y);
            }
        }

        /// <summary>
        /// 現在のPing位置をマーカー表示する。
        /// </summary>
        public void UpdateCursorPosition(double x, double y)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateCursorPosition(x, y)));
                return;
            }

            m_currentCursor = (x, y);

            ClearCursorInternal();

            m_cursorFeatureSet = new FeatureSet(FeatureType.Point);
            //_cursorFeatureSet.AddFeature(new Coordinate(x, y));
            m_cursorFeatureSet.AddFeature(new NetTopologySuite.Geometries.Point(x, y));

            m_cursorLayer = map1.Layers.Add(m_cursorFeatureSet) as IMapPointLayer;
            if (m_cursorLayer != null)
            {
                m_cursorLayer.Symbolizer = new PointSymbolizer(Color.OrangeRed, PointShape.Ellipse, 10f);
                m_cursorLayer.LegendText = "Cursor";
            }

            map1.Refresh();
        }

        /// <summary>
        /// カーソルマーカーを非表示にする。
        /// </summary>
        public void ClearCursor()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(ClearCursor));
                return;
            }

            m_currentCursor = null;
            ClearCursorInternal();
            map1.Refresh();
        }

        private void ClearTrackInternal()
        {
            if (m_trackLayer != null)
            {
                map1.Layers.Remove(m_trackLayer);
                m_trackLayer = null;
            }

            if (m_trackFeatureSet != null)
            {
                m_trackFeatureSet.Dispose();
                m_trackFeatureSet = null;
            }
        }

        private void ClearCursorInternal()
        {
            if (m_cursorLayer != null)
            {
                map1.Layers.Remove(m_cursorLayer);
                m_cursorLayer = null;
            }

            if (m_cursorFeatureSet != null)
            {
                m_cursorFeatureSet.Dispose();
                m_cursorFeatureSet = null;
            }
        }
    }
}
