# Testeontratos

Tecnologia: .net Core 3.1 com Entity Framework
Swagger implementado.
Configurações: 
  Ativar / Desativar o uso de cache de resposta das consultas de contrato
  atrvés da configuração "CacheAtivo": false/true no arquivo appsettings.json do projeto Contratos.
Features:
    Cache: Ao estar ativo as consultas de contatos são armazenadas no cache até as 23:59:59 do mesmo dia.
Rotas:
  1 - v1/Contratos
  Metodos
  Get > obtêm todos os contratos sem detalhar as prestações, o cache funciona para este método caso esteja ativo.
  Post > Cria um contrato
         Exemplos Formato Json
            {
              "dataContratacao": "2021-04-14T12:34:46.326Z",
              "quantidadeParcelas": 3,
              "valorFianciado": 1200
            }
            
            ou
            {
              "quantidadeParcelas": 3,
              "valorFianciado": 1200
            }
        * Quando dataContratacao não é informada será assumida a data/hora atual.
  2 - /v1/contratos/{id}
      Metodo Get > obtêm um contratos através do id da rota detalhando as prestações, o cache funciona para este método caso esteja ativo.
      Metodo Delete > Exclui o contrato e suas parcelas, caso exista no cache exclui do cache também.
  
  3 - /v1/prestacoes/{id}/baixar
      Metodo Post  > Atualiza a data do pagamento de uma parcela a partir de seu id 
        Exemplo de formato no corpo do post: "2021-04-14T12:45:00.231Z"

4 - /v1/prestacoes/contrato/{id}
    Metodo Get > Obtêm uma lista de prestações de um contrato onde o id da rota é o id do contrato.
               [
                {
                  "id": 1,
                  "dataVencimento": "2021-05-14T12:34:46.326Z",
                  "dataPagamento": "0001-01-01T00:00:00",
                  "valor": 400,
                  "statusPrestacao": "Aberta",
                  "contratoId": 1,
                  "contrato": {
                    "id": 1,
                    "dataContratacao": "2021-04-14T12:34:46.326Z",
                    "quantidadeParcelas": 3,
                    "valorFianciado": 1200,
                    "prestacoes": []
                  }
                },
                {
                  "id": 2,
                  "dataVencimento": "2021-06-13T12:34:46.326Z",
                  "dataPagamento": "0001-01-01T00:00:00",
                  "valor": 400,
                  "statusPrestacao": "Aberta",
                  "contratoId": 1,
                  "contrato": {
                    "id": 1,
                    "dataContratacao": "2021-04-14T12:34:46.326Z",
                    "quantidadeParcelas": 3,
                    "valorFianciado": 1200,
                    "prestacoes": []
                  }
                },
                {
                  "id": 3,
                  "dataVencimento": "2021-07-13T12:34:46.326Z",
                  "dataPagamento": "0001-01-01T00:00:00",
                  "valor": 400,
                  "statusPrestacao": "Aberta",
                  "contratoId": 1,
                  "contrato": {
                    "id": 1,
                    "dataContratacao": "2021-04-14T12:34:46.326Z",
                    "quantidadeParcelas": 3,
                    "valorFianciado": 1200,
                    "prestacoes": []
                  }
                }
              ]
      
      Projeto de testes.
      1 - Teste de criação de contrato
      2 - Teste de alteração de parcela
      3 - Teste de status das parcelas
      
