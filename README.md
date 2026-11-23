# PropertyManagerFL

[English](https://github.com/fauxtix/PropertyManagerFL/blob/master/README-EN.md)

Aplicação concebida para ajudar os senhorios na gestão das suas propriedades e alugueres. Com uma interface de fácil utilização e uma série de funcionalidades úteis, o PropertyManagerFL (PMFL) funciona como uma plataforma centralizada para os senhorios, para agilizar as suas tarefas de gestão de propriedades.

## Resumo (comece aqui)

- **O que é:** aplicação localhost de utilizador único, com **API ASP.NET Core** e **UI Blazor Server** que têm de correr **em simultâneo**. Gere imóveis, inquilinos, contratos, rendas, despesas, cartas, mensagens e um **centro de notificações** (sino + caixa de entrada, SignalR com polling de 60 s).
- **Tecnologias:** C#, .NET 10 LTS, Blazor, **Syncfusion 35.2.12**, Dapper, AutoMapper, FluentValidation, Serilog, SQL Server.
- **Pré-requisitos:** SDK .NET 10 (`global.json`), **SQL Server Developer ou Community** (gratuitos) com a base de dados criada, SSMS/`sqlcmd`, certificado HTTPS de desenvolvimento, credenciais SMTP e **as suas próprias licenças Syncfusion**.
- **Base de dados:** criação manual — não há EF migrations ativas. Os scripts (`PropertyManagerFL.Infrastructure/Database/*.sql` ou `SqlData_Script/PropertyManagerDB.sql`) não são idempotentes e devem correr **numa BD vazia**, pela ordem descrita em [DATABASE.md](docs/DATABASE.md).
- **Chaves:** a UI e a API registam cada uma a sua licença Syncfusion. Em Development usam `dotnet user-secrets set --project ...`; em Production usam GitHub Secrets. **Escolher uma chave para a versão 35.2.x e com todas as edições** — detalhes em [SYNCFUSION-LICENSES.md](docs/SYNCFUSION-LICENSES.md).
- **Arranque local:** API em `http://localhost:4300`, UI em `https://localhost:4310`. Ver [LOCAL-SETUP.md](docs/LOCAL-SETUP.md) para os comandos completos.
- **Quem faz clone:** os GitHub Secrets **não vêm com o clone**. Cada pessoa precisa da sua BD, das suas credenciais e das suas licenças. Nunca reutilizar chaves do mantenedor nem as committed.
- **Produção:** deploy manual via GitHub Actions para IIS, com segredos injetados no `web.config` — ver [PRODUCTION.md](docs/PRODUCTION.md).
- **Testes:** `dotnet test PropertyManagerFL.Tests/PropertyManagerFL.Tests.csproj`.

### Guias

| Guia | O que explica |
|---|---|
| [DATABASE.md](docs/DATABASE.md) | Instalar SQL Server, criar `PropertyManagerDB`, ordem dos scripts, verificação |
| [LOCAL-SETUP.md](docs/LOCAL-SETUP.md) | Portas, user-secrets por projeto, arranque, temas/idiomas, resolução de problemas |
| [SYNCFUSION-LICENSES.md](docs/SYNCFUSION-LICENSES.md) | **Qual chave escolher (versão/edição)**, onde configurá-la e como resolver avisos de licença |
| [PRODUCTION.md](docs/PRODUCTION.md) | GitHub Secrets, runner IIS, pools, deploy, rotação de chaves |

## Principais características

- **Informação centralizada sobre propriedades e inquilinos**: O PMFL permite que os proprietários armazenem todos os detalhes da propriedade e do inquilino num único local conveniente. Desde especificações de propriedades a registos de inquilinos e informações de contacto, tudo é facilmente acessível sempre que precisar.

- **Criação de contratos de arrendamento**: O PMFL simplifica o processo de criação de contratos de arrendamento (novos e existentes).

- **Controlo de pagamentos de rendas**: O PMFL fornece um sistema abrangente de acompanhamento de pagamentos de rendas, ajudando os senhorios a manterem-se actualizados sobre as transacções e a tratarem prontamente de quaisquer pagamentos em atraso.

- **Centro de notificações**: sino e caixa de entrada com alertas operacionais (rendas, contratos, documentos). As atualizações chegam por SignalR, com polling de 60 segundos como alternativa.

- **Acompanhamento das despesas do imóvel alugado**: como proprietário, ficará perdido (mesmo com um contabilista) se não acompanhar corretamente as despesas do imóvel alugado.
  Manter registos detalhados das despesas não só o ajudará a sentir-se mais organizado, como também facilitará o preenchimento dos seus impostos, permitindo-lhe ver mais oportunidades de deduções e compreender o retorno de cada um dos seus investimentos em arrendamento.

- **Gestão de cartas de inquilinos**:

  >Convite à renovação - Dá ao inquilino algum tempo para decidir se quer renovar o contrato de arrendamento ou sair.

  >Carta de rescisão - assinala o fim do contrato de arrendamento. Pode dever-se ao facto de o inquilino tencionar mudar-se ou à recusa do senhorio em renovar o contrato de arrendamento

  >Aumento da renda - um convite à renovação para que o inquilino possa decidir se quer ficar

  >Alteração das informações de pagamento - para evitar confusões e continuar a receber os pagamentos da renda a tempo, os senhorios devem informar os inquilinos das alterações de pagamento

  >Aviso de renda em atraso - para colocar avisos de renda por escrito. Um senhorio pode necessitar de provas de que um inquilino se atrasou cronicamente no pagamento como base para rescindir o contrato de arrendamento

  >Carta de pagamento ou desistência - aviso sobre rendas não pagas. Exige o pagamento da renda atual e das rendas em atraso até uma determinada data ou será iniciado um processo de despejo

  >Aumentos de renda - o processo é automático para o ano seguinte, a partir da data de início do contrato, ou através de um procedimento manual para cada inquilino.
    Cada uma destas situações pressupõe uma carta a alertar para a alteração

  >Comunicação com os condóminos - mensagens enviadas ou recebidas de/para os condóminos, através da utilização de correio eletrónico

## Idiomas, temas e documentos

A aplicação suporta os idiomas português, inglês, francês e espanhol (`en-US` é o culture por omissão; `AppSettings:Language` é `pt-PT`). Existe um interruptor de tema claro/escuro: o **dark** é o tema por omissão e a escolha fica guardada no browser.

Algumas das tabelas utilizadas na aplicação (principalmente as de **lookup**) necessitarão da intervenção do utilizador, uma vez que o Português foi utilizado como língua nativa para o seu preenchimento/configuração.

O mesmo se aplica à redação das várias cartas enviadas aos inquilinos, uma vez que cada país pode ter diferentes modelos/regras a aplicar, que terão de ser adaptados em conformidade. É um processo simples de efetuar:

1. Utilize o Winword para abrir cada documento modelo utilizado na aplicação (templates / dotx), armazenado na [pasta dos documentos](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Api/Reports/Docs);
2. Copie o seu texto e utilize um tradutor (Google, DeepL, ...) para o adaptar às suas necessidades;
3. Copie o texto traduzido e substitua o texto da carta (salvaguardando os placeholders dos campos variáveis);
4. Termine o processo guardando os documentos actualizados (substituindo os existentes).

Mais detalhes em [LOCAL-SETUP.md](docs/LOCAL-SETUP.md).

## Base de dados

A estrutura (tabelas, *stored procedures*, funções, vistas) está na [pasta de scripts da base de dados](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Infrastructure/Database) e no script completo `SqlData_Script/PropertyManagerDB.sql`. O centro de notificações e as respetivas procedures fazem parte desses scripts — não há um script separado. As migrações EF estão desativadas: a criação da BD é manual e os scripts só devem ser executados numa base vazia. A ordem de execução está em [DATABASE.md](docs/DATABASE.md).

## Testes e deploy

- Testes: `dotnet test PropertyManagerFL.Tests/PropertyManagerFL.Tests.csproj`.
- CI: `.github/workflows/ci.yml` (restore/build/test em .NET 10, sem BD).
- CD: `.github/workflows/cd.yml`, manual (`workflow_dispatch`) a partir de `master`, num runner self-hosted com o label `iis-propertymanager`. Ver [PRODUCTION.md](docs/PRODUCTION.md).

## 🌟 Contribuição

As contribuições para este projeto são bem-vindas! Se encontrar algum problema ou tiver sugestões de melhoria, por favor abra um problema no repositório GitHub: https://github.com/fauxtix/PropertyManagerFL/Issues

Bifurcar (fork) o projeto (https://github.com/fauxtix/PropertyManagerFL/fork)

Crie um branch para a sua modificação (git checkout -b fauxtix/PropertyManagerFL)

Commit (git commit -am 'added a new feature - some files changed')

Push (git push origin fauxtix/PropertyManagerFL)

Criar um novo Pull Request

Mais informações: https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github

Quando contribuir com código, por favor siga o estilo de código existente e submeta um pull request com as suas alterações. Antes de arrancar a aplicação, siga os [guias de configuração](#guias) e nunca commite chaves, connection strings ou dados de inquilinos.

## Licença

O código fonte do site está licenciado sob a licença MIT, que pode ser encontrada no ficheiro [[MIT-LICENSE.txt](https://github.com/fauxtix/PropertyManagerFL/blob/master/MIT-LICENSE.txt)].

## 📞 Contacto

Se tiver alguma dúvida ou precisar de mais assistência, pode contactar o responsável pelo projeto:

- 👨‍💻 Responsável pela manutenção: Fausto Luís
- ✉ Email: fauxtix.luix@hotmail.com
