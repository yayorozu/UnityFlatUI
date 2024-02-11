// 負数:0, 正数:1 に変換
// 条件を見たしているかどうかチェック
#define IS(value) ceil(saturate(value))
// a がb よりも小さいかどうか
#define IS_SMALL(a, b) IS( b - a)

static float PI = 3.14159265358979323846;

#pragma shader_feature _TYPE_DEFAULT _TYPE_OUTLINE _TYPE_SEPARATE

#pragma shader_feature _SHAPE_CIRCLE _SHAPE_POLYGON _SHAPE_STAR _SHAPE_ROUND_STAR _SHAPE_HEART _SHAPE_CROSS _SHAPE_RING _SHAPE_POLAR _SHAPE_SUPERELLIPSE _SHAPE_ARROW

half4 RoundedCorner(float2 uv, half4 baseColor, half4 targetColor, half radius, half width, half height, int flag)
{
    float r = min(width, height) * radius;
    float2 XY = float2(uv.x * width, uv.y * height);
				
    // Calc Distance from each center of circle
    // LeftTop, Center:(r, r)
    float d_lt = (XY.x - r) * (XY.x - r) + (XY.y - r) * (XY.y - r);
    // LeftBot, Center:(r, h - r)
    float d_lb = (XY.x - r) * (XY.x - r) + (XY.y- (height - r)) * (XY.y - (height - r));
    // RightTop, Center:(w - r, r)
    float d_rt = (XY.x - (width - r)) * (XY.x - (width - r)) + (XY.y - r) * (XY.y - r);
    // RightBot, Center:(w - r, h - r)
    float d_rb = (XY.x - (width - r)) * (XY.x - (width - r)) + (XY.y - (height - r)) * (XY.y - (height - r));
				
    d_lt *= (flag & 1 << 2) == 1 << 2;
    d_rt *= (flag & 1 << 3) == 1 << 3;
    d_lb *= (flag & 1 << 0) == 1 << 0;
    d_rb *= (flag & 1 << 1) == 1 << 1;
				
    float isNotCorner = 
        IS(IS_SMALL(r, XY.x) + IS_SMALL(XY.x, (width - r)) - 1) // r < x < 1-r
        + IS(IS_SMALL(r, XY.y) + IS_SMALL(XY.y, (height - r)) - 1); // r < y < 1-r
				
    float left = lerp(
        lerp(1, 0, IS(d_lt - r * r)),
        lerp(1, 0, IS(d_lb - r * r)),
        IS(uv.y > 0.5)
    );
    float right = lerp(
        lerp(1, 0, IS(d_rt - r * r)),
        lerp(1, 0, IS(d_rb - r * r)),
        IS(uv.y > 0.5)
    );
    half v = lerp( 
        lerp(left, right, IS(uv.x > 0.5)),
        1,
        IS(isNotCorner) // r < x < 1-r && r < y < 1-r
    );

    return lerp(baseColor, targetColor, v);
}

half4 FloatToColor(float value)
{
    half3 outlineColor = half3(0, 0, 0);
    half outlineColorData = value;
    outlineColor.r = frac(value) * 10;
    outlineColor.g = floor(outlineColorData) % 1000 / 100;
    outlineColorData = floor(outlineColorData / 1000);
    outlineColor.b = outlineColorData / 100;

    return half4(outlineColor, 1);
}

half4 RoundedCornerFragment(half4 baseColor, float4 uv, float4 uv1)
{
    half radius = uv1.x;
    int flag = (int)(uv1.y * 15);
				
    half width = uv.z;
    half height = uv.w;
				
#ifdef _TYPE_OUTLINE
    half outline = uv1.w;
    half4 outlineColor = FloatToColor(uv1.z);
    
    half4 color = RoundedCorner(uv.xy, half4(0, 0, 0, 0), outlineColor, radius, width, height, flag);

    // Outlineの最低幅
    float r = min(width, height) * outline;

    half2 newUV = half2(
        lerp(-r / width, 1 + r / width, uv.x), 
        lerp(-r / height, 1 + r / height, uv.y) 
    );
    half outlineX = (r / width) / (1 + r / width);
    half outlineY = (r / height) / (1 + r / height);
    // Inner outline
    color = RoundedCorner(newUV, color, baseColor, radius, width - width * outline * 2, height - height * outline * 2, flag);

    color.rgb = lerp(
        color.rgb, 
        outlineColor,
        saturate(
            (frac(uv.x * (1 + outlineX)) < outlineX) + 
            (frac(uv.y * (1 + outlineY)) < outlineY)
        )
    );

    return color;

#elif _TYPE_SEPARATE
				
    half4 color = RoundedCorner(uv, half4(0, 0, 0, 0), baseColor, radius, width, height, flag);

    half ratio = uv1.w;

    half4 separateColor = FloatToColor(uv1.z);

    color.rgb = lerp(
        separateColor,
        color.rgb, 
        IS_SMALL(uv.y, ratio)
    );

    return color;

#else

    return RoundedCorner(uv, half4(0, 0, 0, 0), baseColor, radius, width, height, flag);

#endif
}

float CalculateRingAlpha(float2 uv, float width, float outerStart = 1)
{
    const float2 pos = (uv.xy - float2(0.5, 0.5)) * 2.0;
    const float len = length(pos);
    const float inner = smoothstep(width, width, len);
    const float outer = smoothstep(outerStart, outerStart, len);
    return  inner - outer;
}

half4 CircleGaugeFragmentAlpha(half4 baseColor, float4 uv0, float4 uv1)
{
    float2 pos = (uv0.xy - float2(0.5, 0.5)) * 2.0;
    
    float angle = (atan2(pos.y, pos.x) - atan2(uv1.y, uv1.x)) / (PI * 2);

    if (angle < 0) {
        angle += 1.0;
    }

    half frameWidth = uv1.z;
    half3 frameColor = FloatToColor(uv1.w).rgb;

    float value1 = uv0.z;
    float value2 = uv0.w;
    float width = 1 - frac(value1) * 10;
    float amount = floor(value1) / 100;
    float reverse = frac(value2) * 10;
    float maxLength = floor(value2) / 100;
    if (reverse > 0) {
        angle = 1 - angle;
    }

    amount = lerp(0, amount, maxLength);
    const half cutoff = angle > amount ? 0 : 1;

    // ゲージの部分
    const float gauge = CalculateRingAlpha(uv0.xy, width + frameWidth, 1.0 - frameWidth);
    const float frame = CalculateRingAlpha(uv0.xy, width, 1.0);

    half4 color = 1;
    color.rgb = lerp(frameColor, baseColor.rgb, gauge * cutoff * step(0, frameWidth));
    color.a = lerp(
        frame * (angle > maxLength ? 0 : 1),
        gauge * cutoff,
        step(frameWidth, 0)
    );

    return color;
}

float2 UVtoPolarCoordinates(float2 uv, float strength)
{
    float2 delta = uv - float2(0.5, 0.5);
    delta *= (1 - strength) * 2;
    float radius = length(delta) * 2;
    float angle = atan2(delta.x, delta.y) * 1.0 / 6.28;
    return float2(radius, angle);
}

half CalculateCircleAlpha(float2 uv, float strength)
{
    const half radius = distance(uv.xy, half2(0.5, 0.5));
    return smoothstep(strength, strength - 0.01, radius);
}

// 多角形
half CalculatePolygonAlpha(float2 uv, float strength, int numSides)
{
    uv += - 0.5;
    float radius = length(uv);
    float theta = atan2(uv.x, uv.y) + 2.0 * PI;
    theta = theta % (2.0 * PI / numSides);

    float polygonDistance = cos(PI / numSides) / cos (PI / numSides - theta);
    return  step(radius, polygonDistance * strength); 
}

float CalculateRoundStarAlpha(float2 uv, float strength, int numPoints, float round)
{
    float2 polarCoordinates = UVtoPolarCoordinates(uv, strength);
    
    float rsq = polarCoordinates.x * polarCoordinates.x;
    float angle = 2.0 * PI * polarCoordinates.y;

    float angleOffset = 3.0 * PI / (2.0 * numPoints);
    float angleSink = asin(round);
    float angleSinkCos = asin(round * cos(numPoints * angle));

    float distance = cos(angleSink / numPoints + angleOffset) / cos(angleSinkCos / numPoints + angleOffset);
    
    float distanceSquared = distance * distance;
    
    return 1 - step(distanceSquared, rsq);
}

float CalculateStarAlpha(float2 uv, float strength, int numPoints)
{
    float2 polarCoordinates = UVtoPolarCoordinates(uv, strength);

    const float reciprocalSquare = polarCoordinates.x * polarCoordinates.x;
    const float angle = 2.0 * PI * polarCoordinates.y;
    const float angleOffset = 2.0 * PI / numPoints;
    const float distance = cos(angleOffset) / cos(angleOffset - acos(cos(numPoints * angle)) / numPoints);
    const float distanceSquared = distance * distance;
    
    return 1 - step(distanceSquared, reciprocalSquare);
}

float CalculateHearAlpha(float2 uv, float strength)
{
    uv -= 0.5;
    uv.x = 1.2 * uv.x - sign(uv.x) * uv.y * 0.55;
    return step(length(uv) - strength, 0);
}

float CalculateCrossAlpha(float2 uv, float strength, float width)
{
    float2 p = abs(uv - 0.5);
    p *= (1 - strength) * 2;
    const float rate = (1 - strength * 2) * width / 1.4;

    const float round = uv.x <= 1 - rate
                        && uv.y <= 1 - rate
                        && uv.x >= rate
                        && uv.y >= rate
                            ? 1 : 0;
    
    return step(2, step(min(p.x, p.y), width) + round);
}

float CalculatePolarAlpha(float2 uv, float strength, int numSides, float value)
{
    float2 delta = uv - float2(0.5, 0.5);
    delta *= (1 - strength) * 2;
    const float len = length(delta) * 2;
    const float atan = atan2(delta.y, delta.x);

    int mode = (int)value;

    float f = cos(atan * numSides) * (mode == 0)
        + abs(cos(atan * numSides)) * (mode == 1)
        + (abs(cos(atan * (numSides - 0.5))) * 0.5 + 0.3) * (mode == 2)
        + (abs(cos(atan * 12) * sin(atan * numSides)) * 0.8 + 0.1) * (mode == 3);
    
    return 1.-smoothstep(f, f + 0.02, len);
}

float fastPow(float x, float p)
{
    return exp(p * log(x));
}

float CalculateSuperEllipseAlpha(float2 uv, float blur, float value)
{
    const float _A = 0.4;
    const float _B = 0.4;
    const float _N = value;
    const float _Blur = blur;

    const float a = fastPow(abs((uv.x - 0.5) / _A), _N) + fastPow(abs((uv.y - 0.5) / _B), _N);

    if (a < 1)
    {
        return 1;
    }
    if (_Blur <= 0)
    {
        return 0;
    }
    
    return 1 / fastPow(a, 10 * (1 - _Blur));
}

// 三角形の内側かどうか
bool isPointInsideTriangle(half2 p, half2 v0, half2 v1, half2 v2)
{
    const float denominator = ((v1.y - v2.y) * (v0.x - v2.x) + (v2.x - v1.x) * (v0.y - v2.y));
    const float lambda1 = ((v1.y - v2.y) * (p.x - v2.x) + (v2.x - v1.x) * (p.y - v2.y)) / denominator;
    const float lambda2 = ((v2.y - v0.y) * (p.x - v2.x) + (v0.x - v2.x) * (p.y - v2.y)) / denominator;
    const float lambda3 = 1.0 - lambda1 - lambda2;
    return (lambda1 >= 0.0 && lambda1 <= 1.0 && lambda2 >= 0.0 && lambda2 <= 1.0 && lambda3 >= 0.0 && lambda3 <= 1.0);
}

// 矢印を描画する
float CalculateArrowAlpha(float2 uv, float strength, float arrowWidth, float arrowLineWidth)
{
    float rate = (1 - strength * 2);
    half2 vertex1 = half2(0, 0.5);
    half2 vertex2 = half2(arrowWidth, 1);
    half2 vertex3 = half2(arrowWidth, 0);
    const half2 center = (vertex1 + vertex2 + vertex3) / 3;

    vertex1 -= (vertex1 - center) * rate;
    vertex2 -= (vertex2 - center) * rate; 
    vertex3 -= (vertex3 - center) * rate;

    arrowLineWidth *= strength * 2;
    
    const bool isInsideTriangle = isPointInsideTriangle(uv, vertex1, vertex2, vertex3);

    return isInsideTriangle ||
        (abs(uv.y - 0.5) < arrowLineWidth && uv.x >= arrowWidth - arrowWidth * rate && uv.x <= 1 - rate / 10)
    ? 1 : 0;
}

float CalculateShapeAlpha(float2 uv, float strength, float4 params)
{
    int intValue = (int)(params.w * 50);
    float value = params.x;
#ifdef _SHAPE_CIRCLE
    
    return CalculateCircleAlpha(uv, strength);
#elif _SHAPE_POLYGON
    
    return CalculatePolygonAlpha(uv, strength, intValue);
#elif _SHAPE_STAR
    
    return CalculateStarAlpha(uv, strength, intValue);    
#elif _SHAPE_ROUND_STAR
    
    return CalculateRoundStarAlpha(uv, strength, intValue, value);

#elif _SHAPE_HEART
    return CalculateHearAlpha(uv, strength);

#elif _SHAPE_CROSS

    return CalculateCrossAlpha(uv, strength, value);

#elif _SHAPE_RING

    return CalculateRingAlpha(uv, value);

#elif _SHAPE_POLAR

    return CalculatePolarAlpha(uv, strength, intValue, value);
#elif _SHAPE_SUPERELLIPSE

    return CalculateSuperEllipseAlpha(uv, params.y, value);
#elif _SHAPE_ARROW
    
    return CalculateArrowAlpha(uv, strength, params.x, params.y);
#endif
    return 0;
}

half4 Shapes(float4 baseColor, float4 uv0, float4 uv1)
{
    const half outlineWidth = uv0.z;
    const half3 outlineColor = FloatToColor(uv0.w).rgb;

    const float outlineStrength = 0.5;
    const float strength = 0.5 - outlineWidth;

    const float outlineAlpha = CalculateShapeAlpha(uv0.xy, outlineStrength, uv1);
    const float alpha = CalculateShapeAlpha(uv0.xy, strength, uv1);

    baseColor.a *= outlineAlpha;
    baseColor.rgb = lerp(baseColor.rgb, outlineColor, (1 - alpha) * (1 - step(outlineWidth, 0)));

    return baseColor;
}
