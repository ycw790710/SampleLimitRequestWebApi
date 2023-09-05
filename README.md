# SampleLimitRequestWebApi

# 未完成

## Service基礎效能測試
![外觀](sample-test-console.png)

## 附註

### react vulnerability: nth-check
* https://github.com/facebook/create-react-app/issues/11647
* https://stackoverflow.com/questions/71282206/github-dependabot-alert-inefficient-regular-expression-complexity-in-nth-check
* 未處理

### openapi-generator-cli指令
* 下載
 * https://github.com/OpenAPITools/openapi-generator#13---download-jar
* 指令, 使用http不使用https
 * java -jar openapi-generator-cli.jar generate -i <swagger.json的http網址> -g csharp -o SampleLimitRequestTestConsole.ServerApis
