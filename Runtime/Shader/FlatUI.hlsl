// 負数:0, 正数:1 に変換
// 条件を見たしているかどうかチェック
#define IS(value) ceil(saturate(value))
// a がb よりも小さいかどうか
#define IS_SMALL(a, b) IS( b - a)

static float PI = 3.14159265358979323846;
static float HALF_PI = PI / 2.0;

#pragma shader_feature _TYPE_DEFAULT _TYPE_OUTLINE _TYPE_SEPARATE

#pragma shader_feature _TYPE_CIRCLE _TYPE_HORIZONTAL _TYPE_VERTICAL

#pragma shader_feature _ROUND_SHAPE_ROUND _ROUND_SHAPE_CUT

#pragma shader_feature _SHAPE_CIRCLE _SHAPE_POLYGON _SHAPE_ROUNDED_POLYGON _SHAPE_STAR _SHAPE_HEART _SHAPE_CROSS _SHAPE_RING _SHAPE_POLAR _SHAPE_SUPERELLIPSE _SHAPE_ARROW _SHAPE_CHECK_MARK _SHAPE_MAGNIFYING_GLASS

// 三角形の内側かどうか
bool isPointInsideTriangle(half2 p, half2 v0, half2 v1, half2 v2)
{
    const float denominator = ((v1.y - v2.y) * (v0.x - v2.x) + (v2.x - v1.x) * (v0.y - v2.y));
    const float lambda1 = ((v1.y - v2.y) * (p.x - v2.x) + (v2.x - v1.x) * (p.y - v2.y)) / denominator;
    const float lambda2 = ((v2.y - v0.y) * (p.x - v2.x) + (v0.x - v2.x) * (p.y - v2.y)) / denominator;
    const float lambda3 = 1.0 - lambda1 - lambda2;
    return (lambda1 >= 0.0 && lambda1 <= 1.0 && lambda2 >= 0.0 && lambda2 <= 1.0 && lambda3 >= 0.0 && lambda3 <= 1.0);
}

half4 FloatToColor(float packed)
{
    packed *= 256 * 256 * 256;
    half3 color = half3(0, 0, 0);

    color.b = fmod(packed, 256);
    packed = floor(packed / 256);
    color.g = fmod(packed, 256);
    color.r = floor(packed / 256);
    return half4(color / 255, 1);
}

// 角丸
half RoundedCorner(float2 uv, half radius, half width, half height, int flag)
{
    float r = min(width, height) * radius;
    float2 XY = float2(uv.x * width, uv.y * height);
				
    // Calc Distance from each center of circle
    // LeftTop, Center:(r, r)
    half d_lt = (XY.x - r) * (XY.x - r) + (XY.y - r) * (XY.y - r);
    // LeftBot, Center:(r, h - r)
    half d_lb = (XY.x - r) * (XY.x - r) + (XY.y- (height - r)) * (XY.y - (height - r));
    // RightTop, Center:(w - r, r)
    half d_rt = (XY.x - (width - r)) * (XY.x - (width - r)) + (XY.y - r) * (XY.y - r);
    // RightBot, Center:(w - r, h - r)
    half d_rb = (XY.x - (width - r)) * (XY.x - (width - r)) + (XY.y - (height - r)) * (XY.y - (height - r));
				
    d_lt *= (flag & 1 << 2) == 1 << 2;
    d_rt *= (flag & 1 << 3) == 1 << 3;
    d_lb *= (flag & 1 << 0) == 1 << 0;
    d_rb *= (flag & 1 << 1) == 1 << 1;

    const half isNotCorner = 
        IS(IS_SMALL(r, XY.x) + IS_SMALL(XY.x, (width - r)) - 1) // r < x < 1-r
        + IS(IS_SMALL(r, XY.y) + IS_SMALL(XY.y, (height - r)) - 1); // r < y < 1-r

    const half left = lerp(
        lerp(1, 0, IS(d_lt - r * r)),
        lerp(1, 0, IS(d_lb - r * r)),
        IS(uv.y > 0.5)
    );
    const half right = lerp(
        lerp(1, 0, IS(d_rt - r * r)),
        lerp(1, 0, IS(d_rb - r * r)),
        IS(uv.y > 0.5)
    );
    const half inIn = uv.y >= 0 && uv.y <= 1 && uv.x >= 0 && uv.x <= 1;
    
    const half final = lerp( 
        lerp(left, right, IS(uv.x > 0.5)),
        inIn,
        IS(isNotCorner) // r < x < 1-r && r < y < 1-r
    );

    return final * inIn;
}

// 角カット
half CutCorner(float2 uv, half radius, half width, half height, int flag)
{
    half r = min(width, height) * radius;
    half2 XY = float2(uv.x * width, uv.y * height);

    const half isNotCorner = 
        IS(IS_SMALL(r, XY.x) + IS_SMALL(XY.x, (width - r)) - 1) // r < x < 1-r
        + IS(IS_SMALL(r, XY.y) + IS_SMALL(XY.y, (height - r)) - 1); // r < y < 1-r
    
    const half inIn = uv.y >= 0 && uv.y <= 1 && uv.x >= 0 && uv.x <= 1;

    // LeftTop
    half lt = (flag & 1 << 0) == 1 << 0 ?
        isPointInsideTriangle(XY, half2(0, height - r), half2(r, height), half2(r, height - r)) :
        XY.x <= r && XY.y >= height - r
    ;
    // RightTop
    half rt = (flag & 1 << 1) == 1 << 1 ?
        isPointInsideTriangle(XY, half2(width - r, height - r), half2(width - r, height), half2(width, height - r)) :
        XY.x >= width - r && XY.y >= height - r
    ;
    // LeftBottom
    half lb = (flag & 1 << 2) == 1 << 2 ?
        isPointInsideTriangle(XY, half2(0, r), half2(r, 0), half2(r, r)) :
        XY.x <= r && XY.y <= r
    ;
    // RightBottom
    half rb = (flag & 1 << 3) == 1 << 3 ?
        isPointInsideTriangle(XY, half2(width, r), half2(width - r, 0), half2(width - r, r)) :
        XY.x >= width - r && XY.y <= r
    ;

    half body = lerp(0, inIn, IS(isNotCorner));
 
    return saturate(lt + rt + lb + rb + body) * inIn;
}

half Corner(float2 uv, half radius, half width, half height, int flag)
{
#ifdef _ROUND_SHAPE_ROUND
    return RoundedCorner(uv, radius, width, height, flag);
#elif _ROUND_SHAPE_CUT
    return CutCorner(uv, radius, width, height, flag);
#endif
    return 1;
}

half4 RoundedCornerFragment(half4 baseColor, float4 uv, float4 uv1)
{
    half radius = uv1.x;
    int flag = (int)(uv1.y * 15);
				
    half width = uv.z;
    half height = uv.w;

    baseColor.a *= Corner(uv, radius, width, height, flag);

#ifdef _TYPE_OUTLINE
    half outline = uv1.w;
    half4 outlineColor = FloatToColor(uv1.z);
    
    // Outlineの最低幅
    float r = min(width, height) * outline;
    half2 newUV = half2(lerp(-r / width, 1 + r / width, uv.x), lerp(-r / height, 1 + r / height, uv.y));

    // Inner outline
    half innerAlpha = Corner(newUV, radius, width, height, flag);

    baseColor.rgb = lerp(outlineColor, baseColor.rgb, innerAlpha);

#elif _TYPE_SEPARATE

    half ratio = uv1.w;

    half4 separateColor = FloatToColor(uv1.z);

    baseColor.rgb = lerp(separateColor, baseColor.rgb, IS_SMALL(uv.y, ratio));

#endif
    return baseColor;
}

float CalculateRingAlpha(float2 uv, float width, float outerStart = 1)
{
    const float2 pos = (uv.xy - float2(0.5, 0.5)) * 2.0;
    const float len = length(pos);
    const float inner = step(width, len);
    const float outer = step(outerStart, len);
    return  inner - outer;
}

half4 GaugeCircle(half4 baseColor, float4 uv0, float4 uv1)
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
    float reverse = frac(value1) * 10;
    float amount = floor(value1) / 100;
    float width = 1 - frac(value2) * 10;
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

// 直線
half4 GaugeLine(half4 baseColor, float4 uv0, float4 uv1)
{
    const float value1 = uv0.z;
    const float reverse = frac(value1) * 10;
    const float amount = floor(value1) / 100;

    half frameWidth = uv1.z;
    half3 frameColor = FloatToColor(uv1.w).rgb;
    const half3 backColor = FloatToColor(uv0.w).rgb;

    float width = uv1.x;
    float height = uv1.y;
    const int flag = 15;
    const float radius = 0.5;

    // Outlineの最低幅
    float r = min(width, height) * frameWidth;

    // 内部用のUV
    const half2 innerUV = half2(lerp(-r / width, 1 + r / width, uv0.x), lerp(-r / height, 1 + r / height, uv0.y));

    // Inner
    half innerAlpha = RoundedCorner(innerUV, radius, width, height, flag);

    // frame
    half frameAlpha = RoundedCorner(uv0.xy, radius, width, height, flag);

#if _TYPE_HORIZONTAL
    int index = 0;
#else
    int index = 1;
#endif
    float amountLerp = reverse > 0 ? uv0[index] > 1 - amount : uv0[index] < amount;
    const half3 innerColor = lerp(backColor, baseColor.rgb, amountLerp);
    return half4(lerp(frameColor, innerColor, innerAlpha), frameAlpha);
}

half4 GaugeColor(half4 baseColor, float4 uv0, float4 uv1)
{
#if _TYPE_CIRCLE
    return GaugeCircle(baseColor, uv0, uv1);
#elif _TYPE_HORIZONTAL || _TYPE_VERTICAL
    return GaugeLine(baseColor, uv0, uv1);
#endif
    return 0;
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
half CalculatePolygonAlpha(float2 UV, float strength, int numSides)
{
    strength *= 2;
    float aWidth = strength * cos(PI / numSides);
    float aHeight = strength * cos(PI / numSides);
    float2 uv = (UV * 2 - 1) / float2(aWidth, aHeight);
    uv.y *= -1;
    const float pCoord = atan2(uv.x, uv.y);
    float r = 2 * PI / numSides;
    const float distance = cos(floor(0.5 + pCoord / r) * r - pCoord) * length(uv);
    return saturate((1 - distance) / fwidth(distance));
}

half CalculateRoundedPolygon(float2 UV, float Width, float Height, float Sides, float Roundness)
{
    UV = UV * 2. + float2(-1.,-1.);
    float epsilon = 1e-6;
    UV.x = UV.x / ( Width + (Width==0)*epsilon);
    UV.y = UV.y / ( Height + (Height==0)*epsilon);
    Roundness = clamp(Roundness, 1e-6, 1.);
    float i_sides = floor( abs( Sides ) );
    float fullAngle = 2. * PI / i_sides;
    float halfAngle = fullAngle / 2.;
    float opositeAngle = HALF_PI - halfAngle;
    float diagonal = 1. / cos( halfAngle );
    // 面取りの値
    float chamferAngle = Roundness * halfAngle; // 面取りの角度
    float remainingAngle = halfAngle - chamferAngle; // 残る角度
    float ratio = tan(remainingAngle) / tan(halfAngle); // これは、ポリゴンの三角形の長さと、面取りの中心からポリゴンの中心までの距離の比率です
    // 面取りの弧の中心
    float2 chamferCenter = float2(
        cos(halfAngle) ,
        sin(halfAngle)
    )* ratio * diagonal;
    // 面取りの弧の開始点
    float2 chamferOrigin = float2(
        1.,
        tan(remainingAngle)
    );
    // アル・カーシーの代数学を使用して以下を特定します:
    // 面取りの中心からポリゴンの中心までの距離 (辺 A)
    float distA = length(chamferCenter);
    // 面取りの半径 (辺 B)
    float distB = 1. - chamferCenter.x;
    // 辺 C の基準長さ (つまり面取りの開始点までの距離)
    float distCref = length(chamferOrigin);
    // これにより、面取りされたポリゴンが、UV 空間に合わせてスケーリングされます
    // diagonal = length(chamferCenter) + distB;
    float uvScale = diagonal;
    UV *= uvScale;
    float2 polaruv = float2 (
        atan2( UV.y, UV.x ),
        length(UV)
    );
    polaruv.x += HALF_PI + 2*PI;
    polaruv.x = fmod( polaruv.x + halfAngle, fullAngle );
    polaruv.x = abs(polaruv.x - halfAngle);
    UV = float2( cos(polaruv.x), sin(polaruv.x) ) * polaruv.y;
    // アル・カーシーの代数学に必要とされる角度を計算します
    float angleRatio = 1. - (polaruv.x-remainingAngle) / chamferAngle;
    // ポリゴンの中心から面取りの端までの距離を計算します
    float distC = sqrt( distA*distA + distB*distB - 2.*distA*distB*cos( PI - halfAngle * angleRatio ) );
    
    float chamferZone = ( halfAngle - polaruv.x ) < chamferAngle;
    half alpha = lerp( UV.x, polaruv.y / distC, chamferZone );
    // この出力は、距離フィールドではなくその形状のマスクを作成します
    alpha = saturate((1 - alpha) / fwidth(alpha));
    return alpha;
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

// 回転する四角形
float RotateRectangle(float2 uv, float width, float height, float cngle, float2 center)
{
    const float angleRad = radians(cngle);
    
    const float cosAngle = cos(angleRad);
    const float sinAngle = sin(angleRad);
    
    float2 translatedUV = uv - center;
    
    const float2 rotatedUV = float2(translatedUV.x * cosAngle - translatedUV.y * sinAngle, translatedUV.x * sinAngle + translatedUV.y * cosAngle);
    
    float2 d = abs(rotatedUV * 2) - float2(width, height);
    d = 1 - d / fwidth(d);
    return  saturate(min(d.x, d.y));
}

// 円
float Circle(float2 uv, float radius, float2 center)
{
    const float2 delta = uv - center;
    const float distance = length(delta);
    return step(distance, radius);
}

// 角丸の四角形
float RoundedRectangle(float2 uv, float2 a, float2 b, float width, float left = 1, float right = 1)
{
    const float2 center = (a + b) / 2;
    float2 to = a - b;
    const float angle = atan2(to.y, to.x) * (180 / PI);
    const float dist = distance(a, b);
    float v = RotateRectangle(uv, dist, width, -angle, center);
    v += Circle(uv, width / 2, a) * left;
    v += Circle(uv, width / 2, b) * right;
    return saturate(v);
}

float CalculateCheckMark(float2 uv, float strength, float4 p)
{
    float a = 0;
    const float width = p.x * strength * 2;
    const float2 left = float2(0.2, p.y);
    const float2 right = float2(0.8, p.z);
    const float2 bottom = float2(0.5, 0.2);
    a += RoundedRectangle(uv, left, bottom, width);
    a += RoundedRectangle(uv, right, bottom, width);
    return saturate(a); 
}

float CalculateMagnifyingGlass(float2 uv, float strength, float4 p)
{
    const float ringWidth = lerp(0.5, 0.3, 0.5);
    const float barWidth = lerp(0.05, 0.15, 0.5);
    const float2 center = float2(0.6, 0.4);
    const float2 right = float2(0.85, 0.15);
    const float bar = RoundedRectangle(uv, center, right, barWidth, 0, 0);
    uv += float2(0.1, -0.1);
    const float ring = CalculateRingAlpha(uv, ringWidth, 0.6);
    return saturate(ring + bar);  
}

float CalculateShapeAlpha(float2 uv, float strength, float4 params)
{
    params.w *= 50;
    int intValue = (int)params.w;
    
#ifdef _SHAPE_CIRCLE
    
    return CalculateCircleAlpha(uv, strength);
#elif _SHAPE_POLYGON
    
    return CalculatePolygonAlpha(uv, strength, intValue);

#elif _SHAPE_ROUNDED_POLYGON

    return CalculateRoundedPolygon(uv, strength * 2, strength * 2, intValue, params.x);
#elif _SHAPE_STAR
    
    return CalculateRoundStarAlpha(uv, strength, intValue, params.x);
#elif _SHAPE_HEART
    return CalculateHearAlpha(uv, strength);

#elif _SHAPE_CROSS

    return CalculateCrossAlpha(uv, strength, params.x);

#elif _SHAPE_RING

    return CalculateRingAlpha(uv, params.x);

#elif _SHAPE_POLAR

    return CalculatePolarAlpha(uv, strength, intValue, params.x);
#elif _SHAPE_SUPERELLIPSE

    return CalculateSuperEllipseAlpha(uv, params.y, params.x);
#elif _SHAPE_ARROW
    
    return CalculateArrowAlpha(uv, strength, params.x, params.y);
#elif _SHAPE_CHECK_MARK

    return CalculateCheckMark(uv, strength, params);
#elif _SHAPE_MAGNIFYING_GLASS

    return CalculateMagnifyingGlass(uv, strength, params);
#endif
    return 0;
}

half4 Shapes(float4 baseColor, float4 uv0, float4 uv1)
{
    const half outlineWidth = frac(uv0.z);
    const half3 outlineColor = FloatToColor(uv0.w).rgb;

    const float outlineStrength = 0.5;
    const float strength = 0.5 - outlineWidth;

    const float outlineAlpha = CalculateShapeAlpha(uv0.xy, outlineStrength, uv1);
    const float alpha = CalculateShapeAlpha(uv0.xy, strength, uv1);

    baseColor.a *= outlineAlpha;
    baseColor.rgb = lerp(baseColor.rgb, outlineColor, (1 - alpha) * (1 - step(outlineWidth, 0)));

    // Clip
    baseColor.a *= (outlineAlpha * alpha) * floor(uv0.z) > 0 ? 0 : 1;

    return baseColor;
}
