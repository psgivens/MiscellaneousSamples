module CoordFornoi

open ForonoiSweep
open ForonoiBeach

let coords = 
    [ (160, 317)
      (59, 299)
      (241, 251)
      (87, 185)
      (175, 83)
      (275, 277)
      (386, 325)
      (381, 390)
      (111, 76)
      (75, 201)
      (342, 198)
      (33, 52)
      (190, 342)
      (154, 256)
      (14, 138)
      (152, 51)
      (234, 111)
      (253, 78)
      (127, 37)
      (318, 125) ]

let sorted = coords |> List.sortBy snd



