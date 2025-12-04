Shader "Ultimate 10+ Shaders/BlurEllipse"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _BlurAmount ("Blur Amount", Range(0, 0.03)) = 0.0128
        _EllipseRadius ("Ellipse Radius", Range(0, 1)) = 0.5
        _Smoothness ("Edge Smoothness", Range(0, 0.5)) = 0.1
        _AspectRatio ("Aspect Ratio", Range(0.5, 2)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Cull Back
        ZTest Always
        
        GrabPass { }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 position : POSITION;
                float4 screenPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };
            
            half _BlurAmount;
            half _EllipseRadius;
            half _Smoothness;
            half _AspectRatio;
            fixed4 _Color;
            sampler2D _GrabTexture : register(s0);
            
            v2f vert(appdata input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.screenPos = output.position;
                output.uv = input.uv;
                return output;
            }
            
            // Функция плавного перехода для краев
            half smoothEdge(half distance, half radius, half smoothness)
            {
                return smoothstep(radius + smoothness, radius - smoothness, distance);
            }
            
            half4 frag(v2f input) : SV_Target
            {
                // Преобразуем UV координаты
                float2 uv = input.screenPos.xy / input.screenPos.w;
                uv.x = (uv.x + 1) * 0.5;
                uv.y = 1.0 - (uv.y + 1) * 0.5;
                
                // Нормализуем UV в диапазон [-1, 1] с учетом aspect ratio
                float2 centeredUV = (input.uv - 0.5) * 2.0;
                centeredUV.x *= _AspectRatio;
                
                // Вычисляем расстояние до центра эллипса
                half distanceFromCenter = length(centeredUV);
                
                // Маска для эллипса
                half ellipseMask = smoothEdge(distanceFromCenter, _EllipseRadius, _Smoothness);
                
                // Если за пределами эллипса - полностью прозрачный
                if (ellipseMask <= 0.01) {
                    discard;
                    return half4(0,0,0,0);
                }
                
                // Применяем размытие
                half4 pixel = half4(0,0,0,0);
                int sampleCount = 0;
                
                // Адаптивное размытие - больше выборок ближе к краям
                for (int x = -2; x <= 2; x++) {
                    for (int y = -2; y <= 2; y++) {
                        float2 offset = float2(x, y) * _BlurAmount;
                        half4 sampleColor = tex2D(_GrabTexture, uv + offset);
                        
                        // Вес в зависимости от расстояния от центра
                        float2 sampleCentered = centeredUV + offset;
                        half sampleDistance = length(sampleCentered);
                        half weight = smoothEdge(sampleDistance, _EllipseRadius, _Smoothness);
                        
                        pixel += sampleColor * weight;
                        sampleCount += (weight > 0) ? 1 : 0;
                    }
                }
                
                pixel = (sampleCount > 0) ? (pixel / sampleCount) : pixel;
                
                return pixel * _Color * ellipseMask;
            }
            ENDCG
        }
    }
}