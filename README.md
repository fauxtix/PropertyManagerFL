# PropertyManagerFL

[English](https://github.com/fauxtix/PropertyManagerFL/blob/master/README-EN.md)

Aplicação concebida para ajudar os senhorios na gestão das suas propriedades e alugueres. Com uma interface de fácil utilização e uma série de funcionalidades úteis, o PropertyManagerFL (PMFL) funciona como uma plataforma centralizada para os senhorios, para agilizar as suas tarefas de gestão de propriedades.

## Resumo (comece aqui)

- **O que é:** aplicação localhost de utilizador único, com **API ASP.NET Core** e **UI Blazor Server** que têm de correr **em simultâneo**. Gere imóveis, inquilinos, contratos, rendas, despesas, cartas, mensagens e um **centro de notificações** (sino + caixa de entrada, SignalR com polling de 60 s). Cada aviso pode trazer uma **sugestão de ação** (abrir a ficha do inquilino, registar um pagamento, gerar uma carta, etc.).
- **Tecnologias:** C#, .NET 10 LTS, Blazor, **Syncfusion 35.2.12**, Dapper, AutoMapper, FluentValidation, Serilog, SQL Server.
- **Pré-requisitos:** SDK .NET 10 (`global.json`), **SQL Server Developer ou Community** (gratuitos) com a base de dados criada, SSMS/`sqlcmd`, certificado HTTPS de desenvolvimento, credenciais SMTP e **as suas próprias licenças Syncfusion**.
- **Base de dados:** criação manual — não há EF migrations ativas. **Nenhum script cria a BD**: criar primeiro `PropertyManagerDB` vazia. Depois escolher **um** caminho: os scripts modulares (`PropertyManagerFL.Infrastructure/Database/*.sql`, pela ordem descrita em [DATABASE.md](docs/DATABASE.md)) **ou** o snapshot gerado `SqlData_Script/PropertyManagerDB.sql` (regenerado a 2026-11-26 e sincronizado com os scripts modulares). Ambos contêm só dados de configuração/referência; **dados de teste são da responsabilidade de cada utilizador**.
- **Chaves:** a UI e a API registam cada uma a sua licença Syncfusion. Em Development usam `dotnet user-secrets set --project ...`; em Production usam GitHub Secrets. **Escolher uma chave para a versão 35.2.x e com todas as edições** — detalhes em [SYNCFUSION-LICENSES.md](docs/SYNCFUSION-LICENSES.md).
- **Arranque local (Development, Kestrel):** API em `https://localhost:5001`, UI em `https://localhost:7181` — portas originais do projeto, distintas do IIS (Produção: 4300/4310), para poderem correr ao mesmo tempo. Ver [LOCAL-SETUP.md](docs/LOCAL-SETUP.md) para os comandos completos.
- **Quem faz clone:** os GitHub Secrets **não vêm com o clone**. Cada pessoa cria a sua BD manualmente e precisa das suas credenciais e licenças. O CD publica a aplicação, mas **não cria nem atualiza bases de dados**. Nunca reutilizar chaves do mantenedor nem as committed.
- **Produção:** deploy manual via GitHub Actions para IIS, com segredos injetados no `web.config` — **excepto a connection string**, que vem do `appsettings.Production.json` e aponta para `PropertyManagerDB_Production` — ver [PRODUCTION.md](docs/PRODUCTION.md).
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

- **Centro de notificações**: sino e caixa de entrada com alertas operacionais e uma **sugestão de ação** em cada aviso. Cada alerta é uma tarefa: pode ser **concluído** ou **reaberto**; abrir o aviso apenas segue o link sugerido. Os alertas incluem rendas em atraso, falta de carta de aviso, rendas a vencer em breve, carta de revogação sem resposta e contrato emitido sem documento gerado. As atualizações chegam por SignalR, com polling de 60 segundos como alternativa. Ao arrancar, a API gera automaticamente os alertas pendentes de todos os utilizadores, mesmo numa base nova.

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

### Fim de contrato, histórico e arquivamento

- A grelha de **Arrendamentos** abre em **Activos**; também pode consultar **Histórico** e **Todos** (neste modo, a coluna *Estado* identifica cada contrato). **Terminar contrato** só fica disponível com um contrato ativo selecionado.
- Terminar um contrato é uma operação **transacional**: marca `Ativo=0` e `Data_Saida`, liberta a fração, grava `HistoricoArrendamentos`, mantém todos os registos consultáveis e anexa `Contrato já não está ativo; decisão tomada em <data>` às notas do contrato, do inquilino e do fiador elegível. Não se apagam inquilinos, fiadores nem contratos no fluxo normal.
- O inquilino é arquivado (`Ativo=0`); o fiador só é arquivado se não estiver associado a outro contrato ativo. A dívida não bloqueia o fim do contrato: gera um alerta `LeaseEndedWithDebt` para todos os utilizadores, com sugestão de regularização.
- O diálogo de fim de contrato apresenta um aviso informativo sobre a carta de revogação e os prazos configurados (`PrazoEnvioCartaRevogacao` / `PrazoRespostaCartaRevogacao`). O aviso não bloqueia a operação nem altera a data do contrato.
- O botão de eliminação de inquilinos/fiadores passou a **Arquivar**: marca `Ativo=0`, anexa uma nota e mantém contratos, recibos, documentos, histórico e notificações. Não é possível arquivar quem tenha contrato ativo.
- A geração mensal de rendas só cria movimentos para contratos ativos e dentro do prazo; contratos terminados ficam apenas no histórico.
- `Data_Fim` não é editável no formulário do contrato. A renovação automática apenas gera aviso/confirmação: nunca altera a data silenciosamente. Para atualizar o termo, use **Estender o prazo do contrato**.
- Em bases Dev/test já existentes, execute a migração idempotente `PropertyManagerFL.Infrastructure/Database/EndLeaseAndArchiving.sql` antes de usar esta funcionalidade; instalações frescas já a incluem nos scripts modulares e no snapshot (ver [DATABASE.md](docs/DATABASE.md)). Nunca a execute em Produção sem autorização e backup.

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

A estrutura (tabelas, *stored procedures*, funções, vistas) está na [pasta de scripts da base de dados](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Infrastructure/Database) e no script completo `SqlData_Script/PropertyManagerDB.sql`, regenerado a 2026-11-26 a partir do snapshot limpo + migração de fim de contrato/arquivamento. O centro de notificações e as respetivas procedures fazem parte desses scripts — não há um script separado; a coluna da sugestão de ação já está incluída. O script completo traz apenas dados de referência/configuração (com `ApiKey` e caminhos de backup como *placeholders*); **não traz imóveis, inquilinos nem outros dados de exemplo** — cada utilizador cria os seus com nomes fictícios. As migrações EF estão desativadas: criar a BD vazia primeiro, pois nenhum script contém `CREATE DATABASE`. **Nunca usar estes scripts para criar ou atualizar a base de dados de Produção** — o CD só publica a aplicação; a BD é gerida por quem administra o SQL Server (num deploy local, o próprio utilizador). A ordem de execução e o caminho de atualização de bases antigas estão em [DATABASE.md](docs/DATABASE.md).

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
