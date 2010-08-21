Feature: Developer connects to a server
	In order to use CouchDB for my data persistence layer
	As a developer
	I want to connect to a server

Scenario: Connect to server
	Given I have a CouchDB instance running at http://127.0.0.1:5984
	When I call ConnectTo on CouchClient
	Then the result should be an instance of CouchClient
	And ServerVersion should not be null or empty
