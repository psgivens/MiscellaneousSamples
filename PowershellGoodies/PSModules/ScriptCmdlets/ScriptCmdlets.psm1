
function Get-StringHash
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true)] 
        [String] $String,

        [Parameter(Mandatory=$false)] 
        $HashName = "SHA256"
    ) 

    $StringBuilder = New-Object System.Text.StringBuilder 
    [System.Security.Cryptography.HashAlgorithm]::Create($HashName).ComputeHash([System.Text.Encoding]::UTF8.GetBytes($String))|%{ 
        [Void]$StringBuilder.Append($_.ToString("x2")) 
    } 
    $StringBuilder.ToString() 
}

function Read-DastConfig
{
    [CmdletBinding()]
    [OutputType([int])]
    Param
    (
        [Parameter(Mandatory=$false)] 
        [Alias('LogPath')] 
        [string]$Path='.\DAST.config'
    )
    Process
    {
        Get-Content $Path -Raw | ConvertFrom-Json
    }
}

function Write-DastConfig
{
    [CmdletBinding()]
    [OutputType([int])]
    Param
    (
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)] 
        $Config,

        [Parameter(Mandatory=$false)] 
        [Alias('LogPath')] 
        [string]$Path='.\DAST.config'
    )
    Process
    {
        $Config | ConvertTo-Json | Out-File $Path 
    }
}

<# 
.Synopsis 
   Write-Log writes a message to a specified log file with the current time stamp. 
.DESCRIPTION 
   The Write-Log function is designed to add logging capability to other scripts. 
   In addition to writing output and/or verbose you can write to a log file for 
   later debugging. 
.NOTES 
   Created by: Jason Wasser @wasserja 
   Modified: 11/24/2015 09:30:19 AM   
 
   Changelog: 
    * Code simplification and clarification - thanks to @juneb_get_help 
    * Added documentation. 
    * Renamed LogPath parameter to Path to keep it standard - thanks to @JeffHicks 
    * Revised the Force switch to work as it should - thanks to @JeffHicks 
 
   To Do: 
    * Add error handling if trying to create a log file in a inaccessible location. 
    * Add ability to write $Message to $Verbose or $Error pipelines to eliminate 
      duplicates. 
.PARAMETER Message 
   Message is the content that you wish to add to the log file.  
.PARAMETER Path 
   The path to the log file to which you would like to write. By default the function will  
   create the path and file if it does not exist.  
.PARAMETER Level 
   Specify the criticality of the log information being written to the log (i.e. Error, Warning, Informational) 
.PARAMETER NoClobber 
   Use NoClobber if you do not wish to overwrite an existing file. 
.EXAMPLE 
   Write-Log -Message 'Log message'  
   Writes the message to c:\Logs\PowerShellLog.log. 
.EXAMPLE 
   Write-Log -Message 'Restarting Server.' -Path c:\Logs\Scriptoutput.log 
   Writes the content to the specified log file and creates the path and file specified.  
.EXAMPLE 
   Write-Log -Message 'Folder does not exist.' -Path c:\Logs\Script.log -Level Error 
   Writes the message to the specified log file as an error message, and writes the message to the error pipeline. 
.LINK 
   https://gallery.technet.microsoft.com/scriptcenter/Write-Log-PowerShell-999c32d0 
#> 
function Write-Log 
{ 
    [CmdletBinding()] 
    Param 
    ( 
        [Parameter(Mandatory=$true, 
                   ValueFromPipelineByPropertyName=$true)] 
        [ValidateNotNullOrEmpty()] 
        [Alias("LogContent")] 
        [string]$Message, 
  
        [Parameter(Mandatory=$false)] 
        [ValidateSet("Error","Warn","Info")] 
        [string]$Level="Info"
    ) 
 
    Begin 
    { 
        if (-not $env:DastLogPrefix) { $env:DastLogPrefix=([io.fileinfo]$MyInvocation.PSCommandPath).BaseName }

        # Set VerbosePreference to Continue so that verbose messages are displayed. 
        $VerbosePreference = 'Continue' 

        $DastDateTimeFormat = if (-not $DastDateTimeFormat) { 'yyyy-MM-dd' }
        
        $path = Join-Path -Path $env:CSOD_VARROOT ("\\AppSec\\DastLogs\\{0}-{1}.log" -f @($env:DastLogPrefix, (Get-Date -Format $DastDateTimeFormat)))
    } 
    Process 
    { 
        # If attempting to write to a log file in a folder/path that doesn't exist create the file including the path. 
        if (!(Test-Path $Path)) { 
            Write-Verbose "Creating $Path." 
            $NewLogFile = New-Item $Path -Force -ItemType File 
            } 
 
        else { 
            # Nothing to see here yet. 
            } 
 
        # Format Date for our Log File 
        $FormattedDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss" 
 
        # Write message to error, warning, or verbose pipeline and specify $LevelText 
        switch ($Level) { 
            'Error' { 
                Write-Error $Message 
                $LevelText = 'ERROR:' 
                } 
            'Warn' { 
                Write-Warning $Message 
                $LevelText = 'WARNING:' 
                } 
            'Info' { 
                Write-Verbose $Message 
                $LevelText = 'INFO:' 
                } 
            }         
        
        # Write log entry to $Path 
        "$FormattedDate $LevelText $Message" | Out-File -FilePath $Path -Append 
    } 
    End 
    { 
    } 
}


function Invoke-CsodRequest
{
    [CmdletBinding()]
    [OutputType([int])]
    Param
    (
        # Param1 help description
        [Parameter(Mandatory=$true,
                   ValueFromPipeline=$true,
                   Position=0)]
        [string[]]
        $ServiceUrls,

        # Param2 help description
        [Parameter(Mandatory=$true)]
        [string]
        $ServerBase,

        [Parameter(Mandatory=$true)]
        [HashTable]
        $Headers,

        [Parameter(Mandatory=$true)]
        [string]
        $ContentType,

        [Parameter(Mandatory=$true)]
        [string]
        $ProxyServer,

        [Parameter(Mandatory=$true)]
        [ValidateSet("Get", "Post", "Put", "Patch", "Delete")]
        $Method,

        [Parameter(Mandatory=$false)]
        [string]
        $Body,

        [Parameter(Mandatory=$false)]
        [switch]
        $Rethrow
    )

    Process
    {
        $serviceurls |%{
            try {
                $url = $ServerBase + $_
                Write-Log ("Invoking {0}: {1}" -f $Method, $url)
                if ($body) {
                    $result = Invoke-WebRequest -UseBasicParsing $url -Method $Method -ContentType $ContentType -Headers $headers -Proxy $proxyServer -Body $Body 
                }
                else {
                    $result = Invoke-WebRequest -UseBasicParsing $url -Method $Method -ContentType $ContentType -Headers $headers -Proxy $proxyServer 
                }
                Write-Log ("Status: {0}: {1}: {2}" -f $result.StatusCode, $Method, $url)
                Remove-Variable result
            }
            catch
            {
                $temp = $ErrorActionPreference
                $ErrorActionPreference = 'silentlycontinue'
                Write-Log -Level Error $url
                $ErrorActionPreference = $temp

                if ($Rethrow) { throw }
            }             
        }
    }
}






#### Working with findings
##

Function Add-Sha256Hash {
    Param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)] 
        $Records
    )
    Process{
        $formatstring = "{0}{1}{2}{3}"
        $Records | %{        
            $prehash = $formatstring -f $_.cweid, $_.name, $_.param, $_.url
            $hash = Get-StringHash -String $prehash -HashName SHA256
            $_ | Select -Property *, @{ "Name"="Hash"; Expression={$hash}}
        }
    }
}

<#
.Synopsis
   Opens the ZAP report file in a grid view. 
.DESCRIPTION
   Opens the ZAP report file in a grid view. This is used to select findings so that they may
   be assessesed. 
.EXAMPLE
   $selectedItems = Review-ZapFindings -ZapJsonFile '.\data\ms_2017-03-7 12.40.59.json' -AssessmentFile .\data\assessments.json
.EXAMPLE
   $selectedItems = Review-ZapFindings -ZapJsonFile '.\data\ms_2017-03-7 12.40.59.json' -IgnoreAssessment
#>
Function Select-ZapFindings {
    Param(
        [Parameter(Mandatory=$true)]
        [string]$ZapJsonFile,

        [Parameter(Mandatory=$true, ParameterSetName="IgnoreAssessment")]
        [switch]$IgnoreAssessment,
        
        [Parameter(Mandatory=$true, ParameterSetName="WithAssessment")]
        [string]$AssessmentDBName,

        [Parameter(Mandatory=$true, ParameterSetName="WithAssessment")]
        [ValidateSet("CutmsWebApp", "CutmsApi", "CutmsMs")]
        [string]$FindingsType,

        [Parameter(Mandatory=$false)]
        [string]$DomainFilter 
    )
    Begin{
        if (-not $IgnoreAssessment) {
            $assessments = Get-ZapAssessments -AssessmentDbName $AssessmentDBName -FindingsType $FindingsType
            $hashes = $assessments | Select -ExpandProperty Hash
        }
        switch($FindingsType.ToLower()){
            "cutmswebapp" { 
                $typeId = "1"
                $dFilter = "csod.com" 
            }
            "cutmsapi"    { 
                $typeId = "2" 
                $dFilter = "csod.com" 
            }
            "cutmsms"     { 
                $typeId = "3" 
                $dFilter = "laxstgexmt" 
            }
        }
        if ($DomainFilter) { $dFilter = $DomainFilter }

    }
    Process {
        Get-Content -raw $ZapJsonFile | ConvertFrom-Json | Set-Variable findings
        
        if ($IgnoreAssessment) {
            $findings.alerts `
                | Where { $_.Risk -eq "Medium" -or $_.Risk -eq "High" } `
                | Add-Sha256Hash `
                | Out-GridView -OutputMode Multiple 
        } else {                               
            $findings.alerts `
                | Where { $_.Url -match $dFilter } `
                | Where { $_.Risk -eq "Medium" -or $_.Risk -eq "High" } `
                | Add-Sha256Hash `
                | Where { $hashes -notcontains $_.Hash } `
                | Out-GridView -OutputMode Multiple 
        }
    }
}

<#
.Synopsis
   Creates an assessment for the records passed in.
.DESCRIPTION
   Creates an assessment for the records passed in. The assessement can either be a Jira ticket or a reason to consider this a false
   positive. The results will be written to the supplied assessment file. 
.EXAMPLE
   $selectedItems | Update-Update-ZapAssessement -AssessmentFile .\data\assessments.json -JiraTicket "CSOD-12347"

.EXAMPLE
    $selectedItems | Update-ZapFindings -AssessmentFile .\data\assessments.json -FalsePositive @"
The error does not contain sensitive info.
"@
#>
Function Update-ZapAssessment {
    Param(
        # Records to be assessed
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)] 
        $Records,

        # File which holds the assessments
        [Parameter(Mandatory=$true)]
        [string]$AssessmentDbName,

        # Jira ticket if one is filed
        [Parameter(Mandatory=$true,
            ParameterSetName="Jira")]
        [string]$JiraTicket,
        
        # Reason to consider this a false positive
        [Parameter(Mandatory=$true,
            ParameterSetName="FalsePositive")]
        [string]$FalsePositive,

        [Parameter(Mandatory=$true)]
        [ValidateSet("CutmsWebApp", "CutmsApi", "CutmsMs")]
        [string]$FindingsType
    )
    Begin{
        $typeId = switch($FindingsType.ToLower()){
                        "cutmswebapp" { "1" }
                        "cutmsapi"    { "2" }
                        "cutmsms"     { "3" }
                    }
    }
    Process {
        $Records `
            | Select -Property cweid, name, param, url, Hash `
            | Select *, @{Name="Ticket"; Expression={ $JiraTicket }} `
            | Select *, @{Name="FalsePositiveMessage"; Expression={$FalsePositive}} `
            | %{ @{
                   TypeId=$typeId;
                   CweId= $_.CweId;
                   Name=$_.Name;
                   Param=$_.Param;
                   Url=$_.Url;
                   Hash=$_.Hash;
                   Ticket=$_.Ticket;
                   FalsePositiveMessage=$_.FalsePositiveMessage
            } }| Invoke-SqlQuery -Sql "[appsec-ast].[dbo].[SP_DAST_InsertAssessment]" `
                -ConnectionString ('Server={0};Integrated Security=True' -f $AssessmentDbName)  `
                -CommandType StoredProcedure 
    }
}

Function Get-ZapAssessments {
    Param(
        # File which holds the assessments
        [Parameter(Mandatory=$true)]
        [string]$AssessmentDbName,

        [Parameter(Mandatory=$true)]
        [ValidateSet("CutmsWebApp", "CutmsApi", "CutmsMs")]
        [string]$FindingsType
    )
    Begin{
        $typeId = switch($FindingsType.ToLower()){
                        "cutmswebapp" { "1" }
                        "cutmsapi"    { "2" }
                        "cutmsms"     { "3" }
                    }
    }
    Process {
        Invoke-SqlQuery -Sql "[appsec-ast].[dbo].[SP_DAST_GetFindingsAssessments]" `
            -ConnectionString ('Server={0};Integrated Security=True' -f $AssessmentDbName) `
            -CommandType StoredProcedure -Parameters @{
                DastType = $typeId
            }
    }
}


Function Publish-FindingsReport {
    Param(
        [Parameter(Mandatory=$true)]
        [string]$ZapJsonFile,

        # File which holds the assessments
        [Parameter(Mandatory=$true)]
        [string]$AssessmentDbName,

        [Parameter(Mandatory=$true)]
        [ValidateSet("CutmsWebApp", "CutmsApi", "CutmsMs")]
        [string]$FindingsType,

        [Parameter(Mandatory=$true)]
        [string]$OutFile,

        [Parameter(Mandatory=$false)]
        [string]$DomainFilter 
    )
    Begin{
        $filestamp = [System.DateTime]::Now.ToString("yyyy-MM-d HH.mm.ss")
        switch($FindingsType.ToLower()){
            "cutmswebapp" { 
                $typeId = "1"
                $dFilter = "csod.com" 
            }
            "cutmsapi"    { 
                $typeId = "2" 
                $dFilter = "csod.com" 
            }
            "cutmsms"     { 
                $typeId = "3" 
                $dFilter = "laxstgexmt" 
            }
        }
        if ($DomainFilter) { $dFilter = $DomainFilter }
    }
    Process {
        Get-Content -raw $ZapJsonFile | ConvertFrom-Json | Set-Variable findings
        $assessments = Get-ZapAssessments -AssessmentDbName $AssessmentDBName -FindingsType $FindingsType
        $hashes = $assessments | Select -ExpandProperty Hash
        $findings.alerts `
            | Where { $_.Url -match $dFilter } `
            | Add-Sha256Hash `
            | %{
                if ($_.Risk -eq "Low") {
                    $_  | Select *, @{Name="Jira"; Expression={""}} `
                        | Select *, @{Name="FalsePositiveMessage"; Expression={""}} `
                }
                else
                {
                    $hash = $_.Hash
                    $assessment = $assessments | Where {$_.Hash -eq $hash} | Select -First 1
                    if (!$assessment) { throw ("Message ID {0} was not assessed" -f $_.messageId) }
                    $_  | Select *, @{Name="Jira"; Expression={$assessment.Jira}} `
                        | Select *, @{Name="FalsePositiveMessage"; Expression={$assessment.FalsePositiveMessage}} 
                    Remove-Variable assessment
                }
            } `
            | Select -Property *, @{Name="timestamp"; Expression={$filestamp}} `
	        | Select -Property ("timestamp", "cweid", "wascid", "pluginId", "name", "risk", "confidence", "url", "attack", "id", "messageId", "param", "reference", "description", "solution", "Jira", "FalsePositiveMessage", "Hash") `
            | Set-Variable results
        $results | export-csv -NoTypeInformation -Delimiter `t -Path $OutFile
    }
}

Function Invoke-SqlQuery {
    param
    ( 
    [Parameter(Mandatory=$false,
                ValueFromPipeline=$true,
                Position=0)]   
    [hashtable]$Parameters=@{},

    [Parameter(Mandatory=$true)]    
    [string]$Sql,

    
    [Parameter(Mandatory=$true)]   
    [string]$ConnectionString,

    [Parameter(Mandatory=$false)]   
    [int]$Timeout=30,

    [Parameter(Mandatory=$false)]
    [ValidateSet("SQL", "StoredProcedure")]
    $CommandType="SQL"
    )

    Begin {    
        $conn = New-Object data.sqlclient.sqlconnection $ConnectionString
        $conn.open()
    }
    Process{
        $cmd = New-Object System.Data.SqlClient.SqlCommand($Sql, $conn)    
        $cmd.CommandTimeout=$Timeout
        if ($CommandType.ToLower() -eq "storedprocedure"){
            $cmd.CommandType = [System.Data.CommandType]::StoredProcedure
        }

        $Parameters.GetEnumerator() |%{

            [Void] $cmd.Parameters.AddWithValue("@"+$_.Key,$_.Value)
        }   
     
        $ds=New-Object system.Data.DataSet
        $da=New-Object system.Data.SqlClient.SqlDataAdapter($cmd)
        $da.fill($ds) | Out-Null
 
        $ds[0].Tables[0]
    }
    End {
        $conn.Close()
    }
}









####################################
# Define Cmdlets
###########################
Function Get-JiraFields($Domain, $basicAuth) {
    Invoke-RestMethod `
        -Uri "$Domain/rest/api/2/field" `
        -Headers @{"Authorization"=$basicAuth} `
        -ContentType "application/json" `
        -Method Get 
}


Function Get-JiraStatuses($Domain, $basicAuth, $ProjectName) {
    $result = Invoke-RestMethod `
        -Uri "$Domain/rest/api/2/project/$ProjectName/statuses" `
        -Headers @{"Authorization"=$basicAuth} `
        -ContentType "application/json" `
        -Method Get 
    $result | Select -ExpandProperty statuses
}



function Select-JiraFields
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true,
                   ValueFromPipeline=$true,
                   Position=0)]
        $FieldNames,

        [Parameter(Mandatory=$true,
                   Position=1)]
        $Fields
    )
    Process
    {
        $Fields | Where {$FieldNames -contains $_.name} 
    }
}


function Get-JiraFieldId
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true,
                   ValueFromPipeline=$true,
                   Position=0)]
        $FieldNames,

        [Parameter(Mandatory=$true,
                   Position=1)]
        $Fields
    )
    Process
    {
        $FieldNames | Select-JiraFields -Fields $Fields | Select -ExpandProperty id
    }
}
function Get-JiraFieldName
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true,
                   ValueFromPipeline=$true,
                   Position=0)]
        $FieldIds,

        [Parameter(Mandatory=$true,
                   Position=1)]
        $Fields
    )
    Process
    {
        $Fields | Where {$FieldIds -contains $_.id} | Select -ExpandProperty name
    }
}

function Resolve-JiraIssue
{
    [CmdletBinding()]
    [OutputType([int])]
    Param
    (
        # Param1 help description
        [Parameter(Mandatory=$true,
                   ValueFromPipeline=$true,
                   Position=0)]
        $Issues,

        # Param2 help description
        [Parameter(Mandatory=$true,
                   Position=1)]
        $Fields,

        # Param2 help description
        [Parameter(Mandatory=$false,
                   Position=1)]
        $Statuses

    )
    Process
    {    
        $issues |%{
            $issue = $_
            $values = @{"key" = $_.Key; "id" = $_.id; "issue"=$_;}
            if ($_.fields.parent) { $values += @{"parent"=$_.fields.parent} }
            if ($_.fields.labels) { $values += @{"labels"=$_.fields.labels} }
            $fields |%{
                $fieldId = $_.id
                $type = $_.schema.type
                $fieldValue = $issue.fields | Select -ExpandProperty $fieldId
                $values += @{
                    $_.name=
                        switch($type) {
                            'string' { $fieldValue }
                            'option' { $fieldValue.value }
                            'user'   { $fieldValue.DisplayName }
                            'date'   { $fieldValue }
                            'option-with-child' { @($fieldValue.value, $fieldValue.child.value) }
                            'status' { $fieldValue.Name } 
                            'array'  { $fieldValue }
#                                ($fieldValue | %{ $_.name }) -join ', ' 
#                                }
                            'any'    { $fieldValue }
                            default { 
                                Write-Host "Undefined type found: " $type ", value: " $fieldValue
                                return $fieldValue }
                        }
                }
            }
            New-Object -TypeName psobject -Property $values
        }
    }
}



Function Get-JiraIssues
{
    [CmdletBinding()]
    [OutputType([int])]
    Param
    (
        [Parameter(Mandatory=$true)]        
        $JQL,
        [Parameter(Mandatory=$true)]        
        $FieldNames,
        [Parameter(Mandatory=$true)]        
        $PropertyNames,
        [Parameter(Mandatory=$false)]        
        $Fields,
        [Parameter(Mandatory=$true)]        
        $BasicAuth,
        [Parameter(Mandatory=$true)]        
        $Domain,
        [Parameter(Mandatory=$false)]        
        $MaxResults = 150,
        [Parameter(Mandatory=$false)]        
        [switch]$DoNotFlatten
    )

    Begin
    {
        if (-not $Fields) { $fields = Get-JiraFields $domain $basicAuth }
        $ffields = $fieldNames | Select-JiraFields -Fields $fields
        $fieldIds = $fieldNames | Get-JiraFieldId -Fields $ffields
        $fieldIds += $propertyNames
    }
    Process
    {
        $body = @{
            "jql"= $JQL;
            "startAt"= 0;
            "maxResults"= $MaxResults;
            "fields"= $fieldIds
        }| ConvertTo-Json 
        $url = "$Domain/rest/api/2/search" 
        $results = Invoke-RestMethod -Uri $url -Headers @{"Authorization"=$basicAuth} -ContentType "application/json" -Method Post -Body $body 
        if ($DoNotFlatten) { $results } else {
            $results.issues | Resolve-JiraIssue -fields $ffields 
        }
    }
}



Export-ModuleMember -Function Read-DastConfig
Export-ModuleMember -Function Write-DastConfig
Export-ModuleMember -Function Invoke-CsodRequest
Export-ModuleMember -Function Write-Log
Export-ModuleMember -Function Get-StringHash
Export-ModuleMember -Function Add-Sha256Hash 

Export-ModuleMember -Function Select-ZapFindings
Export-ModuleMember -Function Update-ZapAssessment
Export-ModuleMember -Function Publish-FindingsReport
Export-ModuleMember -Function Invoke-SqlQuery 

Export-ModuleMember -Function Get-JiraFields
Export-ModuleMember -Function Get-JiraStatuses
Export-ModuleMember -Function Select-JiraFields
Export-ModuleMember -Function Get-JiraFieldId
Export-ModuleMember -Function Get-JiraFieldName
Export-ModuleMember -Function Resolve-JiraIssue
Export-ModuleMember -Function Get-JiraIssues