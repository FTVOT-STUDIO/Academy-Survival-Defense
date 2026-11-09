Shader "DefenseRoguelike/YesodHypnosis"
{
 // RawImage / CanvasRenderer passes its source texture through this standard property.
 Properties { [PerRendererData] _MainTex("UI Texture",2D)="white" {} _WorldTex("Observed World",2D)="black" {} _Power("Power",Float)=0.3 _Complexity("Complexity",Float)=4 _Fracture("Fracture",Float)=0 _Collapse("Collapse",Float)=0 }
 SubShader {
 Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
 Pass {
 Blend SrcAlpha OneMinusSrcAlpha
 ZWrite Off
 ZTest Always
 Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 TEXTURE2D(_WorldTex); SAMPLER(sampler_WorldTex);
 float _Power, _Complexity, _Fracture, _Collapse;
 struct A { float4 pos:POSITION; float2 uv:TEXCOORD0; half4 col:COLOR; };
 struct V { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; half4 col:COLOR; };
 V vert(A a) { V o; o.pos=TransformObjectToHClip(a.pos.xyz);o.uv=a.uv;o.col=a.col;return o; }
 half4 frag(V i):SV_Target {
 float2 p=i.uv*2-1;
 float noise=frac(sin(dot(floor(p*float2(20,60))+floor(_Time.y*22),float2(13.17,73.31)))*43758.5);
 p.x+=(noise-.5)*.075*_Fracture;
 float r=length(p), angle=atan2(p.y,p.x);
 float spiral=step(0,sin(angle*_Complexity+r*(25+_Collapse*65)-_Time.y*(3+_Power*15)));
 float crack=pow(abs(sin(angle*9+sin(angle*17)*2+r*9)),65)*_Fracture;
 float alpha=(1-smoothstep(.8,1,r))*(.55+_Power*.45);
 float c=lerp(spiral,noise,crack);
 if (_Fracture > .1) {
 float2 cell=floor(i.uv*float2(9,6));
 float seed=frac(sin(dot(cell,float2(19.37,71.17)))*43758.5);
 float2 broken=(i.uv-.5)*(1+_Collapse*2)+.5;
 broken+=float2(seed-.5,frac(seed*13)-.5)*_Fracture*.07;
 half3 world=SAMPLE_TEXTURE2D(_WorldTex,sampler_WorldTex,saturate(broken)).rgb;
 float edge=step(.94,max(frac(i.uv.x*9),frac(i.uv.y*6)));
 half3 color=lerp(world,half3(c,c,c),.35+crack*.5)+edge*.5;
 return half4(color,alpha)*i.col;
 }
 return half4(c,c,c,alpha)*i.col;
 }
 ENDHLSL
 }
 }
}
