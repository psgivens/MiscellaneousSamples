open System.IO

type DocType =
    | Ham
    | Spam

let parseDocType = function
    | "ham" -> Ham
    | "spam" -> Spam 
    | _ -> failwith "Unknown label"

let parseLine (line:string) =
    let split = line.Split '\t'
    let label = split.[0] |> parseDocType
    let message = split.[1]
    (label, message)

let fileName = "SMSSpamCollection"
let path = __SOURCE_DIRECTORY__ + @"\Data\" + fileName

let dataset = 
    File.ReadAllLines path
    |> Array.map parseLine

let spamWithFree = 
    dataset 
    |> Array.filter (fun (docType,_) -> docType = Spam)
    |> Array.filter (fun (_,sms) -> sms.Contains "FREE")
    |> Array.length

let hamWithFREE = 
    dataset 
    |> Array.filter (fun (docType,_) -> docType = Ham)
    |> Array.filter (fun (_,sms) -> sms.Contains "FREE")
    |> Array.length

let primitiveClassifier (sms:string) =
    if (sms.Contains "FREE") 
    then Spam
    else Ham

dataset |> Seq.length

#load "NaiveBayes.fsx"
open NaiveBayes.Classifier

tokenizeWords "42 is the Answer to the question";;

let training = 
    dataset 
    |> Seq.skip 1000
    |> Seq.toArray

let validation =
    dataset
    |> Seq.take 1000
    |> Seq.toArray

let txtClassifier = train training tokenizeWords (["txt"] |> set)
txtClassifier "text"

// 
validation 
|> Seq.mapi (fun i (d,s) -> i,d,s)
|> Seq.averageBy (fun (i, docType, sms) ->
//    printfn "%4d procesed %A from %s" i docType sms
    if docType = txtClassifier sms 
    then 1.0 
    else 0.0)
|> printfn "Based on 'txt', correctly classified: %.3f"

// Baseline
validation 
|> Seq.mapi (fun i (d,s) -> i,d,s)
|> Seq.averageBy (fun (i, docType, sms) ->
//    printfn "%4d procesed %A from %s" i docType sms
    if docType = Ham
    then 1.0 
    else 0.0)
|> printfn "Assuming everything is Ham, correctly classified: %.3f"

let vocabulary (tokenize:Tokenizer) (corpus:string seq) =
    corpus 
    |> Seq.map tokenize
    |> Set.unionMany

let allTokens =
    training
    |> Seq.map snd
    |> vocabulary tokenizeWords

let classifyWithAll = train training tokenizeWords allTokens
txtClassifier "text me when you are here"

// 
validation 
|> Seq.mapi (fun i (d,s) -> i,d,s)
|> Seq.averageBy (fun (i, docType, sms) ->
//    printfn "%4d procesed %A from %s" i docType sms
    if docType = classifyWithAll sms 
    then 1.0 
    else 0.0)
|> printfn "Based on 'txt', correctly classified: %.3f"









