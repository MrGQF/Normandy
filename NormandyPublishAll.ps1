
cls

### ���ýű�Ȩ��
### Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

### ����汾��
$publish_version = Read-Host -Prompt 'Input publish version : '
$path = "C:\Code\Auth\Normandy\Source\"
function Change-CsprojVersion
{
    param($CsprojPath, $NewVersion)
    $xml = New-Object XML
    $xml.Load($CsprojPath)
    $element =  $xml.SelectSingleNode("//Version")
    $element.InnerText = $NewVersion
    $xml.Save($CsprojPath)
}


### ������Ҫ�����İ���
$all_project_names = "Normandy.Identity.Domain.Shared",
"Normandy.Infrastructure.Cache","Normandy.Infrastructure.Config", 
"Normandy.Infrastructure.DI","Normandy.Infrastructure.EntityFramework",
"Normandy.Infrastructure.HttpClient","Normandy.Infrastructure.JobSchedule", 
"Normandy.Infrastructure.Log","Normandy.Infrastructure.Mapper",
"Normandy.Infrastructure.Repository","Normandy.Infrastructure.Util" 


### ��������dotnet����
foreach ($project_name in $all_project_names) 
{
    ### �汾��ַ
    $project_path = $path + $project_name + "\" + $project_name + ".csproj"
    
    ### ƽ̨
    $all_platform_configs = "FolderProfile"


    foreach ($platform_config in $all_platform_configs) 
    {  
		
		###
        $publiched_folder = $path + "Normandy.Publish\"
		
		### ���
		dotnet pack  $project_path   -p:PackageVersion=$publish_version --output $publiched_folder
		
		### ����
		$to_be_removed_pdb = $publiched_folder + "\*.pdb"
        Remove-Item $to_be_removed_pdb -Recurse
        
        $to_be_removed_log = $publiched_folder + "\*.log"
        Remove-Item $to_be_removed_log -Recurse
		
        ### ѹ�� 
        
        $zippath = $publiched_folder + ".zip"
        if (Test-Path -Path $zippath)
        {
            Remove-Item $zippath
        }
        $compress = @{
            Path = $publiched_folder + "\*"
            CompressionLevel = "Optimal"
            DestinationPath = $zippath
        }
        Compress-Archive @compress
    }
    
}
