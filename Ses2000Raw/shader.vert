#version 120
varying vec2  vUV;
varying float vOffU;

void main()
{
    gl_Position = ftransform();
    vUV   = gl_MultiTexCoord0.st;
    vOffU = gl_MultiTexCoord1.s;
}
