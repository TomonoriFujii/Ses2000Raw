#version 120
uniform sampler2D uMainTex;
uniform sampler1D uOffsetTex;

varying vec2  vUV;
varying float vOffU;

void main()
{
    float off = texture1D(uOffsetTex, vOffU).r;

    float srcV = vUV.y - off;
    if (srcV <= 0.0 || srcV >= 1.0) {
        discard;
        return;
    }

    vec4 col = texture2D(uMainTex, vec2(vUV.x, srcV));
    gl_FragColor = col;
}
