


// Let's brake this down. 

    let orgList = [ (1, 2); (1, 3); (1, 4); (2, 5); (3, 6); (4, 5); (5, 6); (5, 7) ]
    
// We take your list of tuples and create a functional map of boss to ((report,boss) list)
// This might be known as an adjacency list, which is used for traversing graphs. 

    let orgMap =
        orgList
        |> List.fold (fun acc item -> 
            let key = snd item
            match Map.tryFind key acc with 
            | Some(reports) -> 

// If there is a list of reports under a boss, add to that list

                let map' = Map.remove key acc
                Map.add(key) (item::reports) map'
            | None -> 

// Otherwise, add to an empty list and insert into the dictionary. 

                Map.add(key) (item::[]) acc
            ) Map.empty<int, (int*int) list> 

// Recurse through the items to find all reports.
    
    let findReports supervisor = 
        let rec findReports' acc collection = 
            match collection with 
            | head::tail -> 

// If there is an item, append it to the accumulator

                (findReports' (head::acc) tail) 

// concatenate the current list to the recursive list of reports to reports

                @   match Map.tryFind (fst head) orgMap with
                    | Some(value) -> (findReports' [] value)
                    | None -> []

// If at the end of the list, return the list. 

            | [] -> acc    

// Run the recursive function

        findReports' [] (Map.find supervisor orgMap)    
    
// Run the function. 

    findReports 7

// Return only the reports

    |> List.map fst

// Don't report the report twice. 

    |> List.distinct
