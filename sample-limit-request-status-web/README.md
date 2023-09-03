# 附註

### openapi-generator-cli
https://openapi-generator.tech/docs/generators/typescript/

會是camelCase, 對不上的屬性是undefined
java -jar openapi-generator-cli.jar generate -i http://localhost:5160/swagger/v1/swagger.json -g typescript-fetch -o sample-limit-request-status-web/src/apis/servers/

會是PascalCase, 對上的屬性仍然是undefined..
java -jar openapi-generator-cli.jar generate -i http://localhost:5160/swagger/v1/swagger.json -g typescript-fetch -o sample-limit-request-status-web/src/apis/servers/ --additional-properties=modelPropertyNaming=PascalCase