namespace NumbersTranslators

open NumbersOfBabel
    
type IndonesianNumberTranslator () =
    let digitToText number = 
        match number with
        | 1 -> "satu"
        | 2 -> "dua"
        | 3 -> "tiga"
        | 4 -> "empat"
        | 5 -> "lima"
        | 6 -> "enam"
        | 7 -> "tujuh" 
        | 8 -> "delapan" 
        | 9 -> "sembilan" 
        | _ -> "" // Throw exception
    
    member this.TranslateNumber n = (this :> INumberTranslator).TranslateNumber n
    interface INumberTranslator with
        member this.TranslateNumber n =
            let rec getValue number = 
                let rgetValue f sp sfx= 
                    (f (number / sp)) + sfx + getValue (number % sp)
                match number with
                | 0                 -> ""
                | x when x < 10     -> digitToText number
                | 10                -> "sepuluh"
                | 11                -> "sebelas"
                | x when x < 20     -> (getValue (number % 10))+ " belas"
                | x when x < 100    -> rgetValue digitToText 10  " puluh "
                | x when x < 200    -> "seratus " + getValue (number % 100)
                | x when x < 1000   -> rgetValue digitToText 100 " ratus "
                | _                 -> "Larget than 1000 not supported" // throw exception
            getValue n
        member this.Language 
            with get () = "bahasa Indonesia"

