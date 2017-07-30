

RETURN
# Opening files
$edges = Import-Csv Edges.csv
$nodes = Import-Csv Nodes.csv


# Generates file for Adjacency Matrix
@{ "nodes"= ($nodes | %{ @{ "name"=$_.Label; "group"=[int]$_.Id%2 } })
   "links"= ($edges | %{ @{ "source"=[int]$_.Source; "target"=[int]$_.Target; "value"=1 } })
 } | ConvertTo-Json | Out-File -Encoding ascii "irenesdata.json"

 # Generates the files for Chord Chart
$count = ($nodes | Measure | Select -ExpandProperty Count) -1
[System.Collections.ArrayList]$array = @()
@(0..$count | %{ $array.Add([System.Collections.ArrayList]@(0..$count|%{0})) })
$edges |%{ $array[$_.Source][$_.Target]=1 }
"[`r`n{0}`r`n]" -f (($array |% -Process { "[{0}]" -f ($_ -join ",") }) -join ",`r`n" ) | Out-File -Encoding ascii "matrix.json"
$nodes | Select -Property @{Name="color";Expression={$_.Color.Trim()}}, @{Name="name";Expression={$_.Label.Trim()}} | Export-Csv -Encoding ASCII .\chordnodes.csv -NoTypeInformation




##############################
###### Neo4j #################
##############################


# user is "neo4j"
# password is "secret"
$creds = Get-Credential
#Invoke-RestMethod -Method post -uri "http://localhost:7474/user/neo4j/password" -Credential $creds -Body @"
#{ "password" : "secret" }
#"@ -ContentType "application/json"


# Getting started with Neo4j
Invoke-RestMethod -Credential $creds -Uri "http://localhost:7474/db/data/" -Method Get
Invoke-RestMethod -Uri "http://localhost:7474/user/neo4j" -Method Get -Credential $creds 


# Insert nodes into Neo4j
$index = 97
[char][int]$index
$nodes | %{
#    "CREATE ({3}:Agency {{ id: {0}, label: '{1}', color: '{2}' }})" -f $_.id, $_.label.Trim(), $_.color.Trim(), ([char]$index++)
    $body = @"
{{
  "statements":[{{ "statement":"CREATE (a:Agency {{ id: {0}, label: '{1}', color: '{2}' }})" -f $_.id, $_.label.Trim(), $_.color.Trim() }}]
}}
"@ -f $g['source'].Value, $g['target'].Value, $g['relation'].Value
    Invoke-RestMethod -Credential $creds -Uri "http://localhost:7474/db/data/transaction/commit" -Body $body -Method Post -ContentType "application/json"

}


# Read simplified version of uml file
$lines = Get-Content .\data.uml

# foreach relationship in simplified file, insert a relationship
$lines | %{
    $val = [regex]::Matches($_,"\s*(?<source>\S+)\s*->\s*(?<target>\S+)\s*:\s*(?<relation>\S+)\s*")
    $g = $val[0].Groups
$body = @"
{{
  "statements":[{{ "statement":"MATCH (a:Agency), (b:Agency) WHERE a.label='{0}' AND b.label='{1}' CREATE r=(a)-[:{2}]->(b) RETURN a,b"}}]
}}
"@ -f $g['source'].Value, $g['target'].Value, $g['relation'].Value
    Invoke-RestMethod -Credential $creds -Uri "http://localhost:7474/db/data/transaction/commit" -Body $body -Method Post -ContentType "application/json"
$body
}


















#$edges | Out-GridView
#$edges | %{ 
#    $edge = $_
#    $reciprocal = $edges | Where { $_.Target -eq $edge.Source -and $_.Source -eq $edge.Target } | Select -First 1 
#    $edge | Select *, @{Name="Reciprocal";Expression={ if($reciprocal){$true}else{$false}}}
#} | Out-GridView
#$nodes | Export-Csv -Encoding ASCII nodesplus.csv
#"[`r`n{0}`r`n]" -f (($array |% -Process { "[{0}]" -f ($_ -join ",") }) -join ",`r`n" ) | Out-File -Encoding ascii "matrix.json"