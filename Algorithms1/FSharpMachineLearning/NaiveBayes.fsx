namespace NaiveBayes

module Classifier =

    type Token = string
    type TokenizedDoc = Token Set
    type Tokenizer = string -> TokenizedDoc

    type ClassificationGroup = {   
        Proportion:float
        TokenFrequencies:Map<Token,float> }

    let tokenScore (classification:ClassificationGroup) token =
        match classification.TokenFrequencies.TryFind token with
        | Some(value) -> log value
        | None -> 0.0

    let score document (classification:ClassificationGroup) =
        let scoreToken = tokenScore classification
        log classification.Proportion + (document |> Seq.sumBy scoreToken)

    let classify 
            (groups:(_*ClassificationGroup) seq) 
            (tokenize:Tokenizer)
            (txt:string) =
        let document = tokenize txt
        groups
        |> Seq.maxBy (fun (_, group) -> score document group)
        |> fst

    let proportion count total = float count / float total
    let laplace count total = float (count + 1) / float (total + 1)
    let countContaining token (documents:TokenizedDoc seq) =
        documents 
        |> Seq.filter (Set.contains token)
        |> Seq.length
                
    let analyze 
            (docs:TokenizedDoc seq)
            (totalDocs:int)
            (classificationTokens:Token Set) =
        let docsCount = docs |> Seq.length
        let score token =
            let count = docs |> countContaining token
            laplace count docsCount
        let scoredTokens =
            classificationTokens
            |> Set.map (fun token -> token, score token)
            |> Map.ofSeq
        let groupProportion = proportion docsCount totalDocs
        {   Proportion = groupProportion
            TokenFrequencies = scoredTokens }

    let learn 
            (docs:(_*string)[])
            (tokenize:Tokenizer)
            (classificationTokens:Token Set) =
        let total = docs.Length
        docs 
        |> Seq.groupBy fst
        |> Seq.map (fun (label, docGroup) -> label, docGroup |> Seq.map (snd >> tokenize))
        |> Seq.map (fun (label, groupDocs) -> label, analyze groupDocs total classificationTokens)
        |> Seq.toList

    let train 
            (docs:(_*string)[])
            (tokenize:Tokenizer)
            (classificationTokens:Token Set) =
        let groups = learn docs tokenize classificationTokens
        classify groups tokenize
        
    open System.Text.RegularExpressions
    let matchWords = Regex @"\w+"
    let tokenizeWords (text:string) = 
        text.ToLowerInvariant ()
        |> matchWords.Matches
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.Value)
        |> Set.ofSeq

        
