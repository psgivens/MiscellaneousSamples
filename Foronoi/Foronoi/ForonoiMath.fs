module ForonoiMath

type Coord = int * int
type CircleCoord = Coord * int

let determinant a b c d e f g h i = 
    a*e*i + b*f*g + c*d*h - g*e*c - h*f*a - i*d*b
let square x = x*x
let sumSqr x y = x*x + y*y
let bisector (x',y') (x'',y'') =
    let midx, midy = (x'+x'')/2, (y'+y'')/2
    let s = (y''-y') / (x''-x')
    let m = s/1
    let b = midy - m * midx
    (m,b)

let intercept (m,b) x = 
    let y = m*x + b
    if y >= 0 then  (x,y)
    else            ((0-b)/m, 0)

let distance (x',y') (x'',y'') = 
    sqrt <| square (float x' - x'') + square (float y' - y'')

            (*
        |(x'^2   + y'^2  )   y'    1|
        |(x''^2  + y''^2 )   y''   1|
        |(x'''^2 + y'''^2)   y'''  1|
    h = -----------------------------
                |x'    y'    1|
            2 * |x''   y''   1|
                |x'''  y'''  1|

        |x'    (x'^2   + y'^2  )  1|
        |x''   (x''^2  + y''^2 )  1|
        |x'''  (x'''^2 + y'''^2)  1|
    k = -----------------------------
                |x'    y'    1|
            2 * |x''   y''   1|
                |x'''  y'''  1|

                    | a b c |
    determinant =  | d e f |
                    | g h i |
*)
let calculateCenter (x', y') (x'', y'') (x''', y''') = 
    let ss' = sumSqr x' y'
    let ss'' = sumSqr x'' y''
    let ss''' = sumSqr x''' y'''
    let divisor = 
        2 * determinant 
            x'    y'    1 
            x''   y''   1 
            x'''  y'''  1
            
    let h = 
        let dividend = 
            determinant 
                ss'    y'    1 
                ss''   y''   1 
                ss'''  y'''  1
        (float dividend) / (float divisor)
            
    let k = 
        let dividend = 
            determinant 
                x'    ss'    1  
                x''   ss''   1
                x'''  ss'''  1
        (float dividend) / (float divisor)            
            
    let radius = sqrt <| square (float x' - h) + square (float y' - k)
    let topY = radius + k
    (int h, int k), int topY

