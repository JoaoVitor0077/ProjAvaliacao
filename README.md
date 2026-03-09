**🎓 Sistema de Avaliação Acadêmica**

Sistema web desenvolvido em ASP.NET Core MVC para gerenciamento de disciplinas, matrículas, usuários e avaliações acadêmicas, utilizando Entity Framework Core e ASP.NET Identity para autenticação e controle de acesso.

O projeto foi desenvolvido com foco em boas práticas de arquitetura MVC, separação de responsabilidades e uso de ViewModels, servindo como projeto de estudo e portfólio.

**🚀 Tecnologias Utilizadas**

- C#

- ASP.NET Core MVC

- Entity Framework Core

- SQL Server

- ASP.NET Identity

- LINQ

- Bootstrap

**🏗 Arquitetura do Projeto**

O projeto segue o padrão MVC (Model-View-Controller):

/Models
/ViewModels
/Controllers
/Views
/Data

Separação clara entre:

Models → entidades do banco

ViewModels → comunicação entre controller e view

Controllers → lógica da aplicação

Views → interface do usuário

**👥 Tipos de Usuário**

O sistema possui controle de acesso baseado em Roles (Identity).

**👨‍🎓 Aluno**

- Possui Registro Acadêmico

- Pode visualizar disciplinas matriculadas

- Pode visualizar avaliações disponíveis

**⚙️ Administrador**

Possui acesso total ao sistema:

- Criar usuários

- Editar usuários

- Atribuir roles

- Criar disciplinas

**🔐 Autenticação**

O sistema utiliza ASP.NET Identity, porém com tela de login personalizada.

**⚙️ Funcionalidades Implementadas**

**Usuários**

- Criar usuário

- Editar usuário

- Listar usuários

- Atribuir roles

- Separação por tipo de usuário
    
**Administração**

Tela administrativa com opções para:

- Criar usuários

- Criar disciplinas

**Disciplinas**

- Cadastro de disciplinas

- Cadastro de disciplinas ofertadas

- Associação com professor

**Matrículas**

- Associação entre aluno e disciplina ofertada

- Registro de data

- Status da matrícula

**Avaliações**

- Associadas às disciplinas ofertadas
  
**Tela administrativa com opções para:**

- Criar usuários

- Criar disciplinas

- Disciplinas

- Cadastro de disciplinas

- Cadastro de disciplinas ofertadas

- Associação com professor

- Matrículas

**Avaliações**

- Professores podem criar avaliações

- Associadas às disciplinas ofertadas

**🧪 Seeds do Sistema**

O projeto possui seed inicial para criação de:

- Roles

- Administrador

- Professor

- Aluno

**💾 Banco de Dados**

Banco utilizado:

- SQL Server

**📈 Objetivo do Projeto**

Este projeto foi desenvolvido para:

- Aprimorar conhecimento em ASP.NET Core

- Praticar Entity Framework Core

- Utilizar ASP.NET Identity

- Implementar CRUD completo

- Aplicar boas práticas de arquitetura MVC

**👨‍💻 Autor**

Desenvolvido por **João Vitor Leite de Castro**

**Projeto criado para portfólio e estudos em desenvolvimento .NET.**
