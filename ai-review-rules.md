# Regras de Code Review da Empresa

## Repositórios

- Repositórios não devem possuir regras de negócio
- Todos os métodos devem tratar exceções e apenas realizar o lançamento delas sem interferir nas mensagens de erro ou mesmo gravar qualquer tipo de Log. Exemplo:
```
catch
{
    throw;
}
```
