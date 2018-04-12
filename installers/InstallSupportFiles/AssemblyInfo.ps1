#get the build number form Jenkins env var
    if(!(Test-Path env:\BUILD_NUMBER))
    {
        return
    }

    #set the line pattern for matching
    $linePattern = 'AssemblyFileVersion'
    #get all assemlby info files
    $assemblyInfos = gci -path $env:ENLISTROOT -include AssemblyInfo.cs -Recurse

    #foreach one, read it, find the line, replace the value and write out to temp
    $assemblyInfos | foreach-object -process {
        $file = $_
        write-host -ForegroundColor Green "- Updating build number -- $env:BUILDNUMBER --  in $file"
        if(test-path "$file.tmp")
        {
            remove-item "$file.tmp"
        }
	if($file.IsReadOnly)
	{
		$file.IsReadOnly= $false
	}
        get-content $file | foreach-object -process {
            $line = $_
            if($line -match $linePattern)
            {
                #replace the last digit in the file version to match this build number.
                $line = $line -replace '\d{1,4}.\d{1,4}.\d{1,4}.\d{1,4}"', "$env:BUILDNUMBER`""
		write-host -ForeGroundColor Green "New AssemblyInfo version -- $line"
            }

            $line | add-content "$file.tmp"

        }
        #replace the old file with the new one
        remove-item $file
        rename-item "$file.tmp" $file -Force -Confirm:$false
   }