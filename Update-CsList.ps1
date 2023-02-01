cd (Split-Path ($MyInvocation.MyCommand.Path))

# Get source files
$sourceFiles = ( `
    ls ./src/*.cs -Recurse `
    | % { $_.FullName.Substring((pwd).Path.Length + 1) } `
)

# Scripter.cslist
$sourceFiles > .\Scripter.cslist

# Scripter.csproj
( Get-Content ".\Scripter.csproj" -Raw ) -Replace "(?sm)(?<=^ +<!-- ScripterSource -->`r?`n).*?(?=`r?`n +<!-- /ScripterSource -->)", `
    [System.String]::Join("`r`n", ($sourceFiles | % { "    <Compile Include=`"$_`" />" } ) ) `
| Set-Content ".\Scripter.csproj" -NoNewline

# meta.json
( Get-Content ".\meta.json" -Raw ) -Replace "(?sm)(?<=^  `"contentList`": \[`r?`n).*?(?=`r?`n  \],)", `
    [System.String]::Join("`r`n", ($sourceFiles | % { "    `"Custom\\Scripts\\AcidBubbles\\Scripter\\$($_.Replace("\", "\\"))`"," } ) ).Trim(",") `
| Set-Content ".\meta.json" -NoNewline
