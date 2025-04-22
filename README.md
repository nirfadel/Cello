# Homework Task - Moveo - Task Management

## Overview

  This repository contains the technical Homework for Implementing Task Management api

### Prerequisites
- Git
- Programming language as specified in the assessment (C#)
- Any additional frameworks or libraries mentioned in the assessment instructions
- Visual studio

### Setup Instructions
In the github repo there are some projects and files: 
Main folder: vs solution that includes:
1. TaskManagement.Core - Business project folder project
2. TaskManagement - Api folder project 
3. TaskManagement.Test - Test project
4. 10000 users.docx - this document summarize the architecture for handle 10000 users a day in the system
5. TaskManageDB.bak - sql server backup of task management DB

After cloning the solution, run the it with Visual studio. 
Run it local, it will be at the url - https://localhost:44327

## Assessment Structure
TaskManagement solution:
- Kubernetes:
	- containerization and Kubernetes deployment guidance:
	1. Dockerfile.yml - docker file
	2. docker-compose.yml - docker compose file
	3. Other docker files
- TaskManagement - 
	- controllers:
		1. AuthController - For aws configo authentication
		2. BaseController - Base controller - used for getting common methods (for identity and cliams)
		3. ProjectsController - Projects Crud operations
		4. TasksController - Tasks Crud operations
		5. UserController - User Crud operations - not using it because it managed in aws cognito 
	- appsettings - configuration file includes:
		- Aws cognito configurations
		- ConnectionStrings
		- Serilog
	- Profram.cs - declare and manage all app definations includes DI for services, aws cognito configuration, Serilog, Jwt auth and more.
- TaskManagement.Core - 
	- Authentication - 
		- CognitoSettings - getting all aws cognito settings 
		- CognitoUserService - Cognito user service - not in use for now ( managed at the aws cognito admin)
	- DB - 
		- IUnitOfWork, UnitOfWork - unit of work pattern for encapsulates multiple repository, and to create one responsibility.
		- TaskManageDbContext - DBContext - EntityFramework connection to DB.
	- Extensions -
		- CognitoAuthenticationExtensions - extention for service collection to manage cognito authentication
	- Helper - 
		- Exceptions handle classes
		- AWSHelper - Hash the secret of aws cognito
	- Model - all project models includes DTO for outside use (get, update, create)
- Repository - 
	- Repository, IRepository - generic crud repository
	- ProjectRepository, TaskRepository - imlementation of specific object (project, task)
- Services - 
	- Services that use Dependency injection for seperate of concerns

- TaskManagement.Test project - 
	- CognitoAuthenticationExtensionsTests.cs - some tests for testing the app using Xunit
	
## decisions and patterns used:
  
Backend -
1. using repository pattern for communicate with the db
2. using unit of work for seperate of concerns
3. using entity framework for simplicity 
4. using services on the business for seperate of concerns and using Dependency injection
5. create users on AWS configo for authentication
6. using JWT for creating token based authentication
7. using Serilog for logging - it is better for query of filter the logs ( It supports structured logging - key-value pairs), also it is having advanced features
8. using error handeling - using middleware
9. Implementing pagination in GET requests
10. Implement role-based access control (Policy = "AdminsOnly")
11. Write some unitests using Xunit
12. Include word document for handle 10k users a day


