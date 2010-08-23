Feature: Developer administers databases on server
	In order to create and delete databases
	As a developer
	I want to be able to administer databases on the server

Background:
	Given I have an instance of CouchClient
	And I have a name for a database

Scenario: Create database
	When I call CreateDatabase on CouchClient
	Then the result should be the database was created on the server

Scenario: Delete database
	When I call DeleteDatabase on CouchClient
	Then the result should be the database was deleted on the server
