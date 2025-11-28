using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using NetTopologySuite.Geometries;
using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
//using Coordinate = DotSpatial.Topology.Coordinate;
using Coordinate = NetTopologySuite.Geometries.Coordinate;
using FeatureType = DotSpatial.Data.FeatureType;
//using LineString = DotSpatial.Topology.LineString;
using LineString = NetTopologySuite.Geometries.LineString;
using PointShape = DotSpatial.Symbology.PointShape;

namespace Ses2000Raw
{
    public partial class MapForm : DockContent
    {
        private FeatureSet? m_trackFeatureSet;
        private IMapLineLayer? m_trackLayer;
        private FeatureSet? m_cursorFeatureSet;
        private IMapPointLayer? m_cursorLayer;
        private (double X, double Y)? m_currentCursor;


        private FeatureSet? m_highlightFeatureSet;
        private IMapPointLayer? m_highlightLayer; // 追加
        private IFeature? m_targetFeature;
        private FeatureSet? m_contactFeatureSet;//クリックした点を残すレイヤー
        private IMapPointLayer? m_contactLayer;

        
        

        private BindingList<Anomary> m_anomaryList;
        public BindingList<Anomary> AnomaryList
        {
            get { return m_anomaryList; }
            set { m_anomaryList = value; }
        }

        public MapForm()
        {
            InitializeComponent(); map1.ProjectionModeDefine = ActionMode.Never;
            map1.ProjectionModeReproject = ActionMode.Never;

            //map1.BackColor = Color.Black;
            map1.BackColor = Constant.COMBO_BACKCOLOR;

            //m_anomaryList = new BindingList<Anomary>
            //{
            //    new Anomary {FileName = "1"},
            //    new Anomary {FileName = "2"}
            //};

            m_anomaryList = new BindingList<Anomary>();
            this.dataGridViewAnomary.DataSource = m_anomaryList;
        }
        /// <summary>
        /// Form Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapForm_Load(object sender, EventArgs e)
        {
            // 列幅の自動調整
            this.dataGridViewAnomary.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            this.dataGridViewAnomary.EnableHeadersVisualStyles = false;
            this.dataGridViewAnomary.EnableHeadersVisualStyles = false;

            this.dataGridViewAnomary.BackgroundColor = Constant.BACKCOLOR;
            this.dataGridViewAnomary.GridColor = Color.FromArgb(63, 63, 70);

            this.dataGridViewAnomary.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 51, 55);
            this.dataGridViewAnomary.ColumnHeadersDefaultCellStyle.ForeColor = Constant.FORECOLOR;
            this.dataGridViewAnomary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(62, 62, 64);
            this.dataGridViewAnomary.ColumnHeadersDefaultCellStyle.SelectionForeColor = Constant.FORECOLOR;
            this.dataGridViewAnomary.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.dataGridViewAnomary.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 51, 55);
            this.dataGridViewAnomary.RowHeadersDefaultCellStyle.ForeColor = Constant.FORECOLOR;
            this.dataGridViewAnomary.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(62, 62, 64);
            this.dataGridViewAnomary.RowHeadersDefaultCellStyle.SelectionForeColor = Constant.FORECOLOR;
            this.dataGridViewAnomary.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.dataGridViewAnomary.DefaultCellStyle.BackColor = Constant.BACKCOLOR;
            this.dataGridViewAnomary.DefaultCellStyle.ForeColor = Constant.FORECOLOR;
        }

        /// <summary>
        /// バインドされたデータをリセットし、更新します。
        /// </summary>
        public void UpdateDataGridView()
        {
            // 更新
            //this.bindingSource1.ResetBindings(false);

            if (this.dataGridViewAnomary.Rows.Count == 0) return;
            this.dataGridViewAnomary.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            int iLastRowIndex = this.dataGridViewAnomary.Rows.Count - 1;
            this.dataGridViewAnomary.FirstDisplayedScrollingRowIndex = iLastRowIndex;
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

        private void EnsureContactLayerExists()// FeatureSet を一度作って再利用
        {

            if (m_contactFeatureSet == null)
            {
                m_contactFeatureSet = new FeatureSet(FeatureType.Point);
                m_contactFeatureSet.DataTable.TableName = "Contacts";
            }


            if (m_contactLayer == null)
            {
                m_contactLayer = map1.Layers.Add(m_contactFeatureSet) as IMapPointLayer;//レイヤを一度だけ生成、以降はFeatureSetに追加していく
                if (m_contactLayer != null)
                {
                    m_contactLayer.Symbolizer = new PointSymbolizer(Color.HotPink, PointShape.Ellipse, 8f);
                    m_contactLayer.LegendText = "Contacts";
                }
            }
        }

        // レイヤーを最前面に移動（互換性のため Remove → Add）
        //! 必要なさそうなため、現在は停止中
        private void BringContactLayerToFront()
        {
            if (m_contactFeatureSet == null) return;

            if (m_contactLayer != null && map1.Layers.Contains(m_contactLayer))
            {
                map1.Layers.Remove(m_contactLayer);
            }

            m_contactLayer = map1.Layers.Add(m_contactFeatureSet) as IMapPointLayer;
            if (m_contactLayer != null)
            {
                // シンボライザは再設定しておく（毎回再設定しても小コスト）
                m_contactLayer.Symbolizer = new PointSymbolizer(Color.HotPink, PointShape.Ellipse, 8f);
                m_contactLayer.LegendText = "Contacts";
            }
        }

        public void AddClickedCurSor(int ping, AnalysisForm analysisForm)
        {
            //back groundで動かないとは思うけど
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => AddClickedCurSor(ping, analysisForm)));
                return;
            }

            if (!m_currentCursor.HasValue) return;

            var (x, y) = m_currentCursor.Value;

            // レイヤーの確認、存在しなければ作成
            EnsureContactLayerExists();

            if (!m_contactFeatureSet.DataTable.Columns.Contains("PingNumber"))
                m_contactFeatureSet.DataTable.Columns.Add("PingNumber", typeof(int));

            // FeatureSet に点を追加　レイヤー
            m_contactFeatureSet.AddFeature(new NetTopologySuite.Geometries.Point(x, y)).DataRow["PingNumber"] = ping; ;

            // 必要なら常に最前面へ
            //BringContactLayerToFront();

            // 画面反映（大量追加時はバッチ化して最後に一度だけ呼ぶ）
            HighlightSinglePoint(ping);//追加した点をハイライト表示  
            //todo ここに行の選択を入れたい

            map1.Refresh();
            analysisForm.ToolStripButtonAddContactChecked = false;
        }



        public void HighlightSinglePoint(int targetPingNumber)
        {
            if (m_contactFeatureSet == null || map1 == null) return;

            RemoveHighlightLayer();

            m_highlightFeatureSet = new FeatureSet(FeatureType.Point);//? これそのままでいいのかわからない。レイヤで削除されるが再生成はここ？
            if (m_highlightFeatureSet.DataTable != null)
            {
                m_highlightFeatureSet.DataTable.Clear();
                m_highlightFeatureSet.Features.Clear();
            }

            // 1. 該当Featureを検索
            m_targetFeature = null;//毎回一点のみを参照したいため、nullでリセット
            foreach (var feature in m_contactFeatureSet.Features)
            {
                var pingObj = feature.DataRow?["PingNumber"];
                if (pingObj != null && Convert.ToInt32(pingObj) == targetPingNumber)//object型なので変換
                {
                    m_targetFeature = feature;
                    break;
                }
            }
            if (m_targetFeature == null) return;

            // 2. 新しいFeatureSetを作成し、該当Featureのみ追加
            m_highlightFeatureSet.DataTable.Columns.Add("PingNumber", typeof(int));
            var newFeature = m_highlightFeatureSet.AddFeature(m_targetFeature.Geometry);
            newFeature.DataRow["PingNumber"] = m_targetFeature.DataRow["PingNumber"];

            // 3. 新しいレイヤーを追加し、色を変える
            m_highlightLayer = map1.Layers.Add(m_highlightFeatureSet) as IMapPointLayer;
            if (m_highlightLayer != null)
            {
                m_highlightLayer.Symbolizer = new PointSymbolizer(Color.Yellow, PointShape.Ellipse, 12f);
                // m_highlightLayer.LegendText = "Highlight"; // ←不要
            }

            map1.Refresh();
        }

        private void DeleteSelectedPoint()
        {
            DialogResult result = MessageBox.Show("選択された行を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                // 「はい」が押されたときの処理
                m_contactFeatureSet.Features.Remove(m_targetFeature);
                RemoveHighlightLayer();
                //RenumberReflectionNames(dataGridViewAnomary);//追加順番を保ったまま、途中行が削除されても数字を振りなおす機能

                int deletePingInfo = dataGridViewAnomary.SelectedRows[0].Index;
                dataGridViewAnomary.Rows.RemoveAt(deletePingInfo);
                dataGridViewAnomary.ClearSelection();

                m_targetFeature = null; // 利用後はリセット
            }
        }

        private void RemoveHighlightLayer()//高頻度で呼ばれそうなのでメソッド化
        {
            if (m_highlightLayer != null && map1.Layers.Contains(m_highlightLayer))
            {
                map1.Layers.Remove(m_highlightLayer);
                m_highlightLayer.Dispose();
                m_highlightLayer = null;
                map1.Refresh();
            }
        }

        private void dataGridViewPingInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                Anomary obj = (Anomary)dataGridViewAnomary.Rows[e.RowIndex].DataBoundItem;
                HighlightSinglePoint(obj.PingNo);
            }
            catch (Exception ex)
            {
                RemoveHighlightLayer();
                return;
            }
        }

        private void dataGridViewAnomaryInfo_SelectionChanged(object sender, EventArgs e)
        {
            var grid = dataGridViewAnomary;
            // 複数行選択されている場合は選択解除
            if (grid.SelectedRows.Count > 1)//本当にこれでいいの？
            {
                grid.ClearSelection();
                RemoveHighlightLayer();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null) return;

            switch (e.ClickedItem.Tag.ToString())
            {
                case "csv":
                    ExportCSVProcess();
                    break;
                case "Delete":
                    DeleteProcess();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Export CSV Process
        /// </summary>
        private void ExportCSVProcess()
        {
            if (this.dataGridViewAnomary.Rows.Count == 0)
            {
                MessageBox.Show("エクスポート対象のデータがありません。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string dir = Properties.Settings.Default.RawDir + @"\Anomary";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using (var sfd = new SaveFileDialog()
            {
                Filter = "CSVファイル|*.csv",
                InitialDirectory = dir,
                FileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + "Anomary.csv"
                //AutoUpgradeEnabled = false


            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string filePath = sfd.FileName;
                        using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                        {

                            // ヘッダー行
                            var headers = this.dataGridViewAnomary.Columns.Cast<DataGridViewColumn>()
                                                                           .Select(col => "\"" + col.HeaderText.Replace("\"", "\"\"") + "\"");
                            sw.WriteLine(string.Join(",", headers));
                            // データ行
                            foreach (DataGridViewRow row in this.dataGridViewAnomary.Rows)
                            {
                                if (row.IsNewRow) continue; // 新規行はスキップ
                                var cells = row.Cells.Cast<DataGridViewCell>()
                                    .Select(cell => "\"" + (cell.Value?.ToString().Replace("\"", "\"\"") ?? "") + "\"");
                                sw.WriteLine(string.Join(",", cells));
                            }
                            MessageBox.Show("CSV保存が完了しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(
                            $"ファイルが他のプロセスによって使用中のため保存できません。\n\n{ex.Message}",
                            "ファイルアクセスエラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"CSV保存中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DeleteProcess()
        {
            if (dataGridViewAnomary.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する行を選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int rowIndex = dataGridViewAnomary.SelectedRows[0].Index;

            Anomary obj = (Anomary)dataGridViewAnomary.Rows[rowIndex].DataBoundItem;
            HighlightSinglePoint(obj.PingNo); // ハイライト表示
            DeleteSelectedPoint(); // 削除処理
            map1.Refresh();
        }
    }

}
