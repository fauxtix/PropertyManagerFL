# PropertyManagerFL

[English](https://github.com/fauxtix/PropertyManagerFL/blob/master/README-EN.md) 

Aplicação concebida para ajudar os senhorios na gestão das suas propriedades e alugueres. Com uma interface de fácil utilização e uma série de funcionalidades úteis, o PropertyManagerFL (PMFL) funciona como uma plataforma centralizada para os senhorios, para agilizar as suas tarefas de gestão de propriedades.

# Principais características

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

# Localização / globalização
A aplicação suporta os idiomas português, inglês, francês e espanhol.

Algumas das tabelas utilizadas na aplicação (principalmente as de **lookup**) necessitarão da intervenção do utilizador, uma vez que o Português foi utilizado como língua nativa para o seu preenchimento/configuração.

O mesmo se aplica à redação das várias cartas enviadas aos inquilinos, uma vez que cada país pode ter diferentes modelos/regras a aplicar, que terão de ser adaptados em conformidade. É um processo simples de efetuar:

1. Utilize o Winword para abrir cada documento modelo utilizado na aplicação (templates / dotx), armazenado na [pasta dos documentos](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Api/Reports/Docs);
2. Copie o seu texto e utilize um tradutor (Google, DeepL, ...) para o adaptar às suas necessidades;
3. Copie o texto traduzido e substitua o texto da carta (salvaguardando os placeholders dos campos variáveis);
4. Termine o processo guardando os documentos actualizados (substituindo os existentes).

# Base de dados
Relativamente à estrutura da base de dados (SQl Server), pode aceder aos scripts (tabelas, stored procedures, funções, ...) na [pasta de scripts da base de dados](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Infrastructure/Database). O centro de notificações usa a tabela `dbo.Notifications` e as procedures `usp_Notifications_*`, já incluídas em `WorkingTables.sql` e `StoredProcedures.sql` (e no script completo `SqlData_Script/PropertyManagerDB.sql`). As migrações EF estão desativadas — a criação da base de dados é manual.
#

**Tecnologias utilizadas**: C#, Blazor com componentes Syncfusion, Dapper (Orm), AutoMapper, FluentValidation, Serilog, ... (.NET 10 LTS).

# Configuração local (.NET 10)

O SDK está fixado em `global.json` (10.0.404, `rollForward: latestPatch`). A aplicação destina-se a uma única utilização em `localhost` e a **API e a UI têm de estar a correr ao mesmo tempo**.

1. **Base de dados**: criar `PropertyManagerDB` com os scripts em `PropertyManagerFL.Infrastructure/Database` (ou restaurar `SqlData_Script/PropertyManagerDB.sql`) num SQL Server local e sobrepor a connection string com a ligação local.
2. **Portas**: API `http://localhost:4300`; UI `https://localhost:4310`. O `BaseUrl` da UI é `http://localhost:4300/api` e já vem nos `appsettings.Development.json` / `appsettings.Production.json`; os wrappers, o SignalR e o upload derivam todos dele.
3. **Segredos locais** — nunca os incluir no repositório. Cada projeto tem o seu próprio `UserSecretsId`, pelo que os valores têm de ser definidos **por projeto** com `dotnet user-secrets set --project <proj> "<chave>" "<valor>"`:

   | Projeto | Chaves obrigatórias em Development |
   |---|---|
   | `PropertyManagerFL.Api` | `Serilog:ConnectionStrings:PMConnection`, `ApiKey`, `SyncfusionLicenseKey` (chave da API), `JWT:Secret` (mín. 32 caracteres), `JWT:Issuer`, `JWT:Audience`, `JWT:ExpiraEmMinutos` |
   | `PropertyManagerFL.UI` | `ApiKey` (**mesmo valor** da API), `SyncfusionLicenseKey` (**chave da UI, diferente da API**), `EmailConfiguration:Username`, `EmailConfiguration:SmtpServer`, `EmailConfiguration:Port`, `EmailConfiguration:Password`, `EmailConfiguration:From` |

   A UI exige a secção `EmailConfiguration` no arranque; a API valida `JWT:Secret` no arranque (não pode começar por `<`).
4. Em cada terminal, definir `$env:ASPNETCORE_ENVIRONMENT = 'Development'` antes de arrancar as aplicações. Em `Production` os user-secrets **não** são carregados.
5. Arrancar em dois terminais:
   ```
   dotnet run --project PropertyManagerFL.Api
   dotnet run --project PropertyManagerFL.UI
   ```
   API em `http://localhost:4300`; UI em `https://localhost:4310`.
6. Verificação rápida: `GET http://localhost:4300/api/Imoveis/TableHasData` com o cabeçalho `ApiKey` deve devolver `200` / `true`; a UI deve redirecionar para a página de login e o sino de notificações deve aparecer após autenticação.

# Chaves, segredos e deploy

- **Quem faz clone**: os GitHub Secrets deste repositório **não são visíveis nem copiados pelo clone**. Cada pessoa precisa dos seus próprios user-secrets locais, da sua base de dados, das suas credenciais SMTP e das **suas próprias licenças Syncfusion**. Não reutilize chaves do mantenedor.
- **Syncfusion**: a API e a UI usam licenças **diferentes**. Se aparecer a mensagem `Claim your FREE account and get a key in less than a minute`, a licença daquele projeto está ausente ou inválida — obtenha a sua chave em <https://www.syncfusion.com/account/licenses> e defina `SyncfusionLicenseKey` no respetivo projeto (`--project PropertyManagerFL.Api` ou `--project PropertyManagerFL.UI`).
- **Produção**: o deploy é feito por `.github/workflows/cd.yml` (apenas `workflow_dispatch` a partir de `master`) num runner self-hosted registado **neste repositório** com o label `iis-propertymanager`. O workflow lê os GitHub Secrets e o `deploy-iis.ps1` escreve-os no `web.config` publicado como variáveis de ambiente, porque em Production os user-secrets não são carregados.

  | GitHub Secret | Destino |
  |---|---|
  | `PM_API_KEY` | API e UI (mesmo valor) |
  | `PM_SYNCFUSION_LICENSE` | Licença Syncfusion da **UI** |
  | `PM_SYNCFUSION_LICENSE_API` | Licença Syncfusion da **API** |
  | `PM_PMCONNECTION` | Connection string SQL |
  | `PM_JWT_SECRET`, `PM_JWT_ISSUER`, `PM_JWT_AUDIENCE`, `PM_JWT_EXPIRA_EM_MINUTOS` | API (arranque valida o segredo) |
  | `PM_EMAIL_USERNAME`, `PM_EMAIL_SMTPSERVER`, `PM_EMAIL_PORT`, `PM_EMAIL_PASSWORD`, `PM_EMAIL_FROM` | UI (arranque exige a secção de email) |
  | `PM_UI_CERT_THUMBPRINT` | Opcional: certificado existente para a binding HTTPS 4310 (senão é criado um self-signed) |

- **Atualizar uma chave em produção** (é necessário acesso de escrita aos segredos do repositório):
  ```
  gh secret set PM_SYNCFUSION_LICENSE --repo fauxtix/PropertyManagerFL --body "<nova chave>"
  gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master
  ```
  Substituir `PM_SYNCFUSION_LICENSE` por `PM_SYNCFUSION_LICENSE_API` para a API. Se faltar algum segredo, o deploy falha com `Missing deployment value for <chave>`.
- ** IIS**: os app pools precisam de acesso ao SQL Server (a connection string usa Integrated Security). Neste servidor reutilizam-se os pools `PropertyManagerApi` e `PropertyManagerFL`, que já têm login e `db_owner` na base de dados. Se criar pools novos, tem de lhes dar acesso à BD.
- Nunca publicar valores de segredos em issues, logs, `backup.md` ou ficheiros versionados; os valores em `appsettings` são apenas placeholders.

# Testes

- `dotnet test PropertyManagerFL.Tests/PropertyManagerFL.Tests.csproj`
- Os testes de integração com a base de dados são apenas de leitura e ficam desativados até definir `PMFL_TEST_CONNECTION` com a ligação para uma **cópia** da base de dados. Nunca apontar essa variável para a base de dados de produção nem a colocar num ficheiro versionado.
- O workflow `.github/workflows/ci.yml` executa restore/build/test em .NET 10 sem necessitar de base de dados.

# Imagens da aplicação

- **Ecrã de entrada**
  #
![Main](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/61fe28f7-9703-4a8b-922a-9b948084db15)

- **Propriedades**
 #
![Properties](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d1cf7d8f-900d-49c3-9065-82a86a92803a)
![Properties_edit_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/13edd0bd-3925-4a16-8011-fb515ffdd239)
![Properties_edit_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/8dbe7cfa-bea7-4260-96e6-75390c6fcfdc)
![Properties_edit_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/135498e8-f373-41d8-bf8f-35d3f6fb5a20)

- **Inquilinos**
  #
![Tenants](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/40d50281-d7ae-4acb-8b75-f40087801743)
![Tenants_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6edc7a70-b197-4e3b-b9e6-f540811d74ba)
![Tenants_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/11dce827-b339-4ac2-a8b8-b72bab92b956)
![Tenants_documents_create](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/ac2ab8e7-fc90-4088-8a82-1adf9c930094)
![Tenants_documents_create_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/3966d0c8-a3ad-4236-a918-671cbf54b938)
![Tenants_documents_create_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/9fb35174-0c4a-4ecd-9105-4f75f91cf465)
![Tenants_guarantor](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d8742713-ecd6-48eb-b800-8eaed7c1cf3f)

- **Frações**
 #
![Unit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/54a0caae-2265-49e6-8b87-e500d43ab15d)
![Unit_Images](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/77df0a32-2d8a-4699-ba78-2b66a8d00279)

- **Pagamentos mensais**
 #
![MonthlyRentPayment_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/67874f25-5fb6-4e66-8cfd-1783c5eb1c29)
![MonthlyRentPayment_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6169ae59-9c2d-4db2-b407-f8e2d7bc6e58)

- **Pagamentos**
  #
![Payments_Dashboard_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d4fe17c6-6977-4c60-b2a2-c5f3e22d7cb3)
![Payments_Dashboard_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/9961bd55-7314-45d0-bb26-f035b3800627)
![Payments](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/cdcc4ce8-ed48-48e9-8241-fe3a4da2770e)
![Payments_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/f904752f-d38a-4cab-ac5a-bad459f6fac0)
![Payments_new_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/48d94d8e-6d17-4ecb-a89d-abc3a73815a7)
![Payments_new_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/0cfa598b-43ff-4398-9d9c-0523cfdaa363)
![Payments_new_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/aaf6cf89-7dca-46e2-a54f-1cf9984b56b4)

- **Contratos de arrendamento**
 #
![Leases](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/b573364b-939b-4f70-be09-acc9634093f5)
![Leases_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/85d07a7a-8c79-4fba-a252-359cf0a1e593)

- **Despesas**
 #
![Expenses_maintenance](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/b164b028-23b8-4b6c-b057-e70d2f7ba095)
![Expenses_maintenance_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6dda062b-02e5-410c-a5ae-bc1bf961677c)
![Expenses_maintenance_subCategories_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/c47278d5-e37e-44d0-9132-1f1cf8ec1d39)
![Expenses_maintenance_subCategories_new](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/7c443597-3d3e-41f3-8251-f84374f5a481)

![Expenses_dashboard_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/66b25d6a-5928-44cd-a489-5d48cd5cde5f)
![Expenses_dashboard_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6e7a0cd7-39d2-4690-9dca-1f7de3c34b5d)
![Expenses_dashboard_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/37b05f59-b63b-47c9-9fbc-4552d6e27fa3)

- **Agenda de marcações**
  #
![Scheduler_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/8419c317-ec2a-472e-8d4a-cb174b62a6f7)

- **Communicação de/para Inquilinos**
  #
![Messages](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/33ed9de2-d80e-418f-bb00-df5d2a15ad32)

- **Contactos**
  #
![Contacts](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/c32585b3-b6b7-4042-bda8-296e79ff76c9)

- **Manutenção**
  #
![Management_CoefAtual](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6e4c2bb9-6f79-46f8-ae8e-5892081c6891)
![Management_Tables_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/ce70eded-fc43-433f-ac1b-116877a6c6e6)
![Management_Tables_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/a1ab269d-dbc2-4a93-a583-153a4f093260)
![Management_LogViewer](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/5b3e38c8-74da-4c9c-b6a5-704329aff130)


## 🌟 Contribuição

As contribuições para este projeto são bem-vindas! Se encontrar algum problema ou tiver sugestões de melhoria, por favor abra um problema no repositório GitHub: https://github.com/fauxtix/PropertyManagerFL/Issues 

Bifurcar (fork) o projeto (https://github.com/fauxtix/PropertyManagerFL/fork)

Crie um branch para a sua modificação (git checkout -b fauxtix/PropertyManagerFL)

Commit (git commit -am 'added a new feature - some files changed')

Push (git push origin fauxtix/PropertyManagerFL)

Criar um novo Pull Request

Mais informações: https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github

Quando contribuir com código, por favor siga o estilo de código existente e submeta um pull request com as suas alterações.

## Licença

O código fonte do site está licenciado sob a licença MIT, que pode ser encontrada no ficheiro [[MIT-LICENSE.txt](https://github.com/fauxtix/PropertyManagerFL/blob/master/MIT-LICENSE.txt)].

## 📞 Contacto

Se tiver alguma dúvida ou precisar de mais assistência, pode contactar o responsável pelo projeto:

- 👨‍💻 Responsável pela manutenção: Fausto Luís
- ✉ Email: fauxtix.luix@hotmail.com

