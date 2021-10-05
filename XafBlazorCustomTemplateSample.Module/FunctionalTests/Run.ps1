$testExecutorPath = "${env:ProgramFiles(x86)}\DevExpress 21.2\Components\Tools\eXpressAppFrameworkNetCore\EasyTest\TestExecutor.v21.2.exe"
$fileName = $args[0]
$fullPath = $PSScriptRoot
if($fileName){
	$fullPath = "$PSScriptRoot\$fileName"
}
& $testExecutorPath $fullPath