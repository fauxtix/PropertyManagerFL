# PropertyManagerFL

Aplicação concebida para ajudar os senhorios na gestão das suas propriedades e alugueres. Com uma interface de fácil utilização e uma série de funcionalidades úteis, o PropertManagerFL (PMFL) funciona como uma plataforma centralizada para os senhorios, para agilizar as suas tarefas de gestão de propriedades.

# Principais características

- **Informação centralizada sobre propriedades e inquilinos**: O PMFL permite que os proprietários armazenem todos os detalhes da propriedade e do inquilino num único local conveniente. Desde especificações de propriedades a registos de inquilinos e informações de contacto, tudo é facilmente acessível sempre que precisar.

- **Criação de contratos de arrendamento**: O PMFL simplifica o processo de criação de contratos de arrendamento, novos e existentes.

- **Controlo de pagamentos de rendas**: O PMFL fornece um sistema abrangente de acompanhamento de pagamentos de rendas, ajudando os senhorios a manterem-se actualizados sobre as transacções e a tratarem prontamente de quaisquer pagamentos em atraso.

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

Algumas das tabelas utilizadas na aplicação (principalmente as de **lookup**) necessitarão da intervenção do utilizador, uma vez que o português foi utilizado como língua nativa para o seu preenchimento/configuração.

O mesmo se aplica à redação das várias cartas enviadas aos inquilinos, uma vez que cada país pode ter diferentes modelos/regras a aplicar, que terão de ser adaptados em conformidade.
É um processo simples de efetuar:
1. Utilize o Winword para abrir cada documento modelo utilizado na aplicação (templates / dotx), armazenado em https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Api/Reports/Docs;
2. Copie o seu texto e utilize um tradutor (Google, DeepL, ...) para o adaptar às suas necessidades;
3. Copie o texto traduzido e substitua o texto da carta (salvaguardando os placeholders dos campos variáveis);
4. Termine o processo guardando os documentos actualizados (substituindo os existentes).

# Base de dados
Relativamente à estrutura da base de dados (SQl Server), pode aceder aos scripts (tabelas, stored procedures, funções, ...) em https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Infrastructure/Database
#

**Tecnologias utilizadas**: C#, Blazor com componentes Syncfusion, Dapper (Orm), AutoMapper, FluentValidation, Serilog, ... (.Net 7).

## Contribuição

As contribuições para este projeto são bem-vindas! Se encontrar algum problema ou tiver sugestões de melhoria, por favor abra um problema no repositório GitHub: https://github.com/fauxtix/PropertyManagerFL/Issues 

Bifurcar o projeto (https://github.com/fauxtix/PropertyManagerFL/fork)

Crie um ramo para a sua modificação (git checkout -b fauxtix/PropertyManagerFL)

Commit (git commit -am 'added a new feature - some files changed')

Fazer push_ (git push origin fauxtix/PropertyManagerFL)

Criar um novo Pull Request

Mais informações: https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github

Quando contribuir com código, por favor siga o estilo de código existente e submeta um pull request com as suas alterações.

## Licença

O código fonte do site está licenciado sob a licença MIT, que pode ser encontrada no ficheiro MIT-LICENSE.txt.

## 📞 Contacto

Se tiveres alguma dúvida ou precisares de mais assistência, podes contactar o responsável pelo projeto:

- 👨‍💻 Mantenedor: Fausto Luís
- ✉ Email: fauxtix.luix@hotmail.com

Sinta-se à vontade para me contactar com qualquer feedback ou perguntas.
