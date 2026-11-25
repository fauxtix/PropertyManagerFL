# Manual de Utilização — Property Manager

> Guia prático da aplicação **Property Manager**: imóveis, frações, inquilinos, contratos de arrendamento, rendas, pagamentos, despesas, documentos, mensagens, agenda e administração.
>
> Destina-se a utilizadores finais e administradores. Não são necessários conhecimentos de programação.
>
> 💡 Este manual está disponível **dentro da aplicação**: clique no botão `?` na **barra superior** para o abrir num diálogo, sem perder a página onde está.

---

## Índice

- [1. Primeiros passos](#primeiros-passos)
- [2. Visão geral e navegação](#navegacao)
- [3. Dashboard](#dashboard)
- [4. Imóveis](#imoveis)
- [5. Frações](#fracoes)
- [6. Inquilinos](#inquilinos)
- [7. Arrendamentos](#arrendamentos)
- [8. Rendas e pagamentos](#rendas-pagamentos)
- [9. Despesas](#despesas)
- [10. Contactos, proprietário e documentos](#contactos-documentos)
- [11. Mensagens, notificações e agenda](#mensagens)
- [12. Modelos de cartas](#modelos-cartas)
- [13. Calculadoras e mapas](#calculadoras)
- [14. Administração](#administracao)
- [15. Resolução de problemas](#resolucao)
- [16. Perguntas frequentes](#faq)
- [Apêndice A — Rotas principais](#rotas)

---

## 1. Primeiros passos {#primeiros-passos}

### 1.1 Entrar na aplicação

1. Abra o endereço da aplicação no browser.
2. Introduza o **email** e a **palavra passe** da sua conta.
3. Clique em **Entrar**.
4. Se ainda não tem conta, use a ligação de **registo** no ecrã de autenticação (em algumas instalações a conta é criada pelo administrador).

### 1.2 Escolher o idioma

1. Na barra superior, utilize o **seletor de idioma**.
2. Escolha entre **Português**, **Inglês**, **Espanhol** e **Francês**.
3. A escolha é guardada nas definições da aplicação e recarrega a interface.

> 💡 O conteúdo que introduziu (nomes, moradas, descrições) **não é traduzido** — apenas a interface. Pode ter a interface em inglês e os dados em português.

### 1.3 Criar o primeiro imóvel

1. No menu lateral, abra **Imóveis** (`/imoveis`).
2. Clique em **Novo** (botão verde com `+`).
3. Preencha os dados de identificação e a **morada** do imóvel.
4. Clique em **Guardar**.

### 1.4 Criar a primeira fração

1. Abra **Imóveis** e selecione o imóvel pretendido.
2. Adicione uma **fração** (unidade) — por exemplo um apartamento ou uma loja.
3. Preencha a identificação da fração, a **renda**, o **tipo** e a **situação**.
4. Pode carregar **imagens** da fração na área de gestão de imagens (`/fracoes/imagesmanager`).

### 1.5 Criar o primeiro inquilino

1. No menu lateral, abra **Inquilinos** (`/inquilinos`).
2. Clique em **Novo** e preencha os dados pessoais e de contacto.
3. Se aplicável, associe um **fiador**.
4. Guarde.

### 1.6 Criar o primeiro arrendamento

1. Abra **Arrendamentos** (`/arrendamentos`).
2. Clique em **Novo**.
3. Associe a **fração** e o **inquilino**, indique a data de início, o prazo e o valor da renda.
4. Guarde. A partir daqui poderá gerar o contrato e as cartas relacionadas.

### 1.7 Tabela «quero fazer X → vou a Y»

| Quero… | Vou a… | Rota |
|---|---|---|
| Ver o resumo geral | Início | `/` |
| Gerir imóveis | Imóveis | `/imoveis` |
| Gerir frações e imagens | Imóveis → fração | `/fracoes/viewunits` |
| Gerir inquilinos | Inquilinos | `/inquilinos` |
| Gerir contratos | Arrendamentos | `/arrendamentos` |
| Processar rendas mensais | Pagamentos → Processar rendas | `/monthlyrentprocessing` |
| Lançar recebimentos | Pagamentos → Manutenção | `/recebimentos/manutencao` |
| Gerir despesas | Despesas → Manutenção | `/despesas/manutencao` |
| Editar cartas | Administration → Modelos de cartas | `/LetterTemplates` |
| Ver notificações | Sino na barra superior | `/notifications` |
| Abrir a agenda | Agenda | `/scheduler` |
| Definições e backups | Manutenção | `/admin/appsettings` |

---

## 2. Visão geral e navegação {#navegacao}

### 2.1 Barra superior

Contém:

- O botão de **abrir/fechar menu lateral**.
- O nome da aplicação.
- O botão de **ajuda** (`?`) — abre este manual sem sair da página.
- O **sino de notificações**.
- O **seletor de idioma**.
- O **nome de utilizador** (ligação para gerir a conta).
- O botão de **tema claro/escuro**.
- A opção de **terminar sessão**.

### 2.2 Menu lateral

O menu está organizado em grupos:

- **Geral** — Início, Imóveis, Inquilinos, Arrendamentos, Despesas, Pagamentos, Calculadoras, Contactos, Documentos, Mensagens e Agenda.
- **Administração/manutenção** — Logs, Definições, Backup da base de dados e Backup de ficheiros.

Os grupos com filhos expandem/recolhem ao clicar. O item ativo fica realçado.

### 2.3 Grelhas

Quase todas as listagens usam uma grelha com barra de ferramentas. As operações mais comuns são:

| Ação | Como |
|---|---|
| Pesquisar | Caixa de pesquisa na barra da grelha |
| Ordenar | Clicar no cabeçalho da coluna |
| Exportar | `PdfExport` ou `ExcelExport` na barra de ferramentas |
| Expandir/recolher linhas | Botões de expandir/recolher na barra |
| Criar | Botão **Novo** |
| Editar / apagar | Botões na linha ou menu de contexto |

> 💡 Em algumas grelhas, o botão direito do rato abre um **menu de contexto** com ações rápidas (cartas de atraso, atualização de renda, etc.).

---

## 3. Dashboard {#dashboard}

A página inicial (`/`) apresenta os indicadores gerais da aplicação: resumo de imóveis, inquilinos, arrendamentos, rendas e despesas, bem como alertas e atividade recente.

- Use os cartões para saltar para a área correspondente.
- Se um número estiver desatualizado, recarregue a página (F5).
- Os **alertas** do dashboard são os mesmos do sino de notificações — não são avisos diferentes por página.

---

## 4. Imóveis {#imoveis}

Página: **Imóveis** (`/imoveis`). Existe também uma vista alternativa em `/properties`.

### 4.1 O que é um imóvel

O **imóvel** é o nível superior: um prédio, uma moradia ou um conjunto. Dentro dele ficam as **frações** (unidades).

### 4.2 Criar e editar

1. Clique em **Novo**.
2. Preencha a identificação e a morada.
3. Guarde.
4. Para alterar, use o botão de edição na linha da grelha.

### 4.3 Exportar e pesquisar

A grelha permite pesquisar, ordenar e exportar a listagem a PDF.

---

## 5. Frações {#fracoes}

Páginas: **Frações** (`/fracoes/viewunits`) e **Gestor de imagens** (`/fracoes/imagesmanager`).

### 5.1 O que é uma fração

Uma **fração** é uma unidade dentro do imóvel — por exemplo um apartamento, uma loja ou uma garagem. É à fração que se associa o **arrendamento** e a **renda**.

### 5.2 Dados principais

- Identificação e descrição.
- **Tipo de fração** e **situação** (por exemplo, disponível ou arrendada).
- **Renda** e **valor de IMI**.
- **Seguro** associado.
- **Ativa / inativa**.

### 5.3 Imagens

1. Abra o gestor de imagens (`/fracoes/imagesmanager`).
2. Selecione a fração.
3. Carregue uma ou mais imagens.
4. Confirme o carregamento; as imagens ficam disponíveis na vista da fração.

> 💡 Se o carregamento falhar com `Access to the path ... is denied`, a pasta de uploads do servidor não tem permissões de escrita — peça ao administrador para aplicar as permissões corretas no IIS.

---

## 6. Inquilinos {#inquilinos}

Página: **Inquilinos** (`/inquilinos`).

### 6.1 Criar um inquilino

1. Clique em **Novo**.
2. Preencha o **nome**, os **contactos** e os dados de identificação.
3. Se existir, associe um **fiador**.
4. Guarde.

### 6.2 A ficha do inquilino

A ficha reúne:

- Dados pessoais e de contacto.
- **Documentos** carregados (`DocumentosInquilino`).
- **Saldo corrente** (`SaldoCorrenteInquilino`) — o que está em dívida ou a crédito.
- Contratos associados.

### 6.3 Atualizar o valor da renda

Existem duas formas:

- **Manual**: no menu de contexto do inquilino, escolha **Atualizar valor da renda** (`/updaterentvalue/{id}`).
- **Por carta**: melalui **Arrendamentos → Atualização de rendas** (`/updaterentsletter`).

### 6.4 Cartas rápidas

O menu de contexto do inquilino permite gerar diretamente:

- **Carta de atraso de pagamento**.
- **Carta de revogação / oposição ao contrato**.
- **Carta de atualização de rendas**.

> ⚠️ Antes de gerar uma carta, confirme que os dados do contrato e do inquilino estão corretos — a carta usa os valores guardados.

---

## 7. Arrendamentos {#arrendamentos}

Página: **Arrendamentos** (`/arrendamentos`).

### 7.1 O que é um arrendamento

O arrendamento liga um **inquilino** a uma **fração**, com data de início, prazo e valor de renda. A aplicação avisa quando um contrato está próximo do fim ou quando há rendas em atraso.

### 7.2 Criar um contrato

1. Clique em **Novo**.
2. Selecione a **fração** e o **inquilino**.
3. Indique **data de início**, **prazo** e **valor da renda**.
4. Guarde.

### 7.3 Ações na grelha

A barra de ferramentas e o menu de contexto permitem:

- **Ver o contrato** (`Visualizar contrato`).
- **Estender o prazo do contrato**.
- **Atualizar rendas** e gerar a carta correspondente.
- Consultar o **saldo corrente do inquilino** (`/saldocorrenteinquilino`).

### 7.4 Coeficientes de atualização

Em **Arrendamentos → Coeficientes** (`/updatecoefficients`) pode gerir os coeficientes usados no cálculo das atualizações anuais de renda.

> 💡 Consoante a configuração da instalação, o menu *Arrendamentos* pode aparecer com submenus (Manutenção e Atualização de rendas) em vez de um único item.

---

## 8. Rendas e pagamentos {#rendas-pagamentos}

Páginas: **Pagamentos → Manutenção** (`/recebimentos/manutencao`), **Processar rendas** (`/monthlyrentprocessing`) e **Dashboard de pagamentos** (`/payments/dashboard`).

### 8.1 Lançar um recebimento

1. Abra **Pagamentos → Manutenção**.
2. Clique em **Novo** (ou equivalente).
3. Selecione o **inquilino**, o **conceito**, o **valor** e a **data**.
4. Guarde.

A grelha permite pesquisar, ordenar e exportar a Excel.

### 8.2 Processamento mensal de rendas

1. Abra **Pagamentos → Processar rendas** (`/monthlyrentprocessing`).
2. Confirme o **mês/ano** e os critérios.
3. Reveja a pré-visualização das rendas a processar.
4. Confirme o processamento.

> ⚠️ O processamento em lote cria vários registos de uma só vez. Reveja sempre a lista antes de confirmar.

### 8.3 Dashboard de pagamentos

O dashboard (`/payments/dashboard`) resume os recebimentos por período e permite acompanhar o que foi pago e o que está pendente.

---

## 9. Despesas {#despesas}

Páginas: **Despesas → Manutenção** (`/despesas/manutencao`), **Tipos de despesa** (`/despesas/tipodespesas`) e **Dashboard de despesas** (`/expenses/dashboard`).

### 9.1 Registar uma despesa

1. Abra **Despesas → Manutenção**.
2. Clique em **Novo**.
3. Preencha a **descrição**, o **valor**, a **data** e o **tipo de despesa**.
4. Guarde.

### 9.2 Tipos de despesa

Em **Tipos de despesa** pode criar, editar e organizar as categorias usadas no lançamento.

### 9.3 Gráficos

O dashboard apresenta gráficos de despesas por tipo e ao longo do tempo (linha e setor). Passe o rato sobre um ponto para ver o valor exato.

---

## 10. Contactos, proprietário e documentos {#contactos-documentos}

### 10.1 Contactos

Página: `/contactos`. Guarda contactos úteis (administradores, condomínios, fornecedores, etc.).

### 10.2 Proprietário

Páginas: `/Proprietario` e `/ProprietarioPage`. Reúnem os dados do proprietário do imóvel.

### 10.3 Documentos

Página: **Documentos** (`/documentos`). Permite registar e carregar documentos gerais da aplicação. Para documentos de um inquilino, use a ficha do inquilino.

---

## 11. Mensagens, notificações e agenda {#mensagens}

### 11.1 Notificações

O **sino** na barra superior abre o centro de notificações. Também pode abrir a página completa em `/notifications`.

- As notificações incluem avisos operacionais (consultas, rendas, cartas, contratos).
- Cada aviso pode trazer uma **sugestão de ação** — por exemplo, abrir a ficha do inquilino, registar um pagamento ou gerar a carta correspondente.
- Os alertas incluem: rendas em atraso, falta de carta de aviso, rendas a vencer em breve, carta de revogação sem resposta e contrato emitido sem documento gerado.
- As mensagens por ler aparecem assinaladas.
- Ao abrir uma notificação, esta é marcada como lida.

### 11.2 Mensagens

Página: **Mensagens** (`/messages-modern`; a versão antiga está em `/messages`). Permite enviar e consultar mensagens.

### 11.3 Agenda

Página: **Agenda** (`/scheduler`). Permite registar compromissos e consultas, com vista de calendário e pesquisa de eventos (`/scheduler/search-events`).

---

## 12. Modelos de cartas {#modelos-cartas}

Página: **Modelos de cartas** (`/LetterTemplates`).

### 12.1 O que são modelos

São ficheiros `.dotx` usados para gerar automaticamente cartas e contratos (avisos de atraso, atualização de rendas, revogação, contrato de arrendamento, etc.).

### 12.2 Editar um modelo

1. Abra **Modelos de cartas**.
2. Selecione o modelo na lista.
3. Edite o documento no editor de texto.
4. Para ver o resultado com valores de exemplo, use **Pré-visualizar**.
5. **Guarde** apenas quando estiver satisfeito.

### 12.3 Campos de mesclagem

Os modelos contêm **campos de mesclagem** (por exemplo `«NomeInquilino»`) que são preenchidos com os dados do contrato/inquilino no momento de gerar a carta. Não apague estes campos sem necessidade.

> ⚠️ A eliminação de um modelo é permanente. Confirme sempre antes de apagar.

---

## 13. Calculadoras e mapas {#calculadoras}

### 13.1 Calculadoras

No menu **Calculadoras** estão disponíveis:

- **IMI** (`/CalculadoraIMI`) — cálculo do Imposto Municipal sobre Imóveis.
- **VPT** (`/CalculadoraVPT`) — cálculo do valor patrimonial tributário.
- **Gestão de coeficientes IMI** (`/ManutencaoCoeficientesIMI`).

### 13.2 Mapas

As páginas `/maps` e `/gmap` mostram a localização dos imóveis num mapa.

---

## 14. Administração {#administracao}

### 14.1 Definições

Página: **Definições** (`/admin/appsettings`). Permite configurar parâmetros gerais da aplicação, como o idioma predefinido e a atualização automática de rendas.

### 14.2 Backups

- **Base de dados** (`/BackupDatabase`) — cria uma cópia da base de dados.
- **Ficheiros/Uploads** (`/backupuploads/1`) — cria cópias das pastas de ficheiros carregados.

> 💡 Faça backups regularmente e guarde as cópias num local diferente do servidor.

### 14.3 Logs

- **Logs da aplicação** (`/admin/viewapplogs`) — erros, avisos e eventos da aplicação.
- **Informação de logins** (`/admin/logininfo`) — registos de entradas na aplicação.

### 14.4 Tabelas e painéis

- **Painel administrativo** (`/admin/adminpanel`) e **configuração de tabelas** (`/admin/configapptables`).
- **Dashboard de resumo** (`/summarydashboard`).

---

## 15. Resolução de problemas {#resolucao}

| Sintoma | Causa provável | O que fazer |
|---|---|---|
| «Acesso negado ao caminho…» ao carregar ficheiros | A pasta de uploads no servidor não tem permissão de escrita | Peça ao administrador para aplicar as permissões no IIS |
| A grelha não carrega | API indisponível ou ligação à base de dados falhada | Verifique os logs (`/admin/viewapplogs`) e tente novamente |
| O manual não abre | Ficheiro `docs/Help*.md` ausente no servidor | Confirme que a pasta `docs` foi publicada com a aplicação |
| Dados «desapareceram» | Filtro ativo na grelha ou utilizador diferente | Limpe os filtros e confirme a conta com que entrou |
| Botões sem resposta | Página carregada antes da ligação Blazor ficar pronta | Aguarde um segundo e tente de novo; recarregue se persistir |

---

## 16. Perguntas frequentes {#faq}

**Preciso de saber programar para usar a aplicação?**
Não. Todo o funcionamento é feito através de formulários e grelhas.

**Posso usar a aplicação noutro idioma?**
Sim. Português, Inglês, Espanhol e Francês. A escolha faz-se no seletor de idioma da barra superior.

**Os meus dados são apagados ao mudar de idioma?**
Não. Apenas a interface muda.

**O que acontece se eu apagar um inquilino com contratos?**
A aplicação avisa ou impede a eliminação, consoante as regras configuradas. Resolva primeiro os contratos associados.

**Como sei se uma renda foi paga?**
Consulte o **saldo corrente do inquilino** e o **dashboard de pagamentos**.

**O manual é atualizado sem reiniciar a aplicação?**
Sim. O ficheiro é lido do disco cada vez que abre o diálogo.

---

## Apêndice A — Rotas principais {#rotas}

| Área | Rota |
|---|---|
| Início | `/` |
| Imóveis | `/imoveis`, `/properties` |
| Frações | `/fracoes/viewunits`, `/fracoes/imagesmanager` |
| Inquilinos | `/inquilinos`, `/Inquilinos/Detalhe` |
| Arrendamentos | `/arrendamentos`, `/updaterentsletter`, `/updatecoefficients` |
| Rendas | `/monthlyrentprocessing`, `/saldocorrenteinquilino` |
| Pagamentos | `/recebimentos/manutencao`, `/payments/dashboard` |
| Despesas | `/despesas/manutencao`, `/despesas/tipodespesas`, `/expenses/dashboard` |
| Contactos | `/contactos` |
| Documentos | `/documentos` |
| Mensagens | `/messages-modern`, `/messages` |
| Notificações | `/notifications` |
| Agenda | `/scheduler` |
| Modelos de cartas | `/LetterTemplates` |
| Calculadoras | `/CalculadoraIMI`, `/CalculadoraVPT`, `/ManutencaoCoeficientesIMI` |
| Mapas | `/maps`, `/gmap` |
| Definições | `/admin/appsettings` |
| Backups | `/BackupDatabase`, `/backupuploads/1` |
| Logs | `/admin/viewapplogs`, `/admin/logininfo` |
