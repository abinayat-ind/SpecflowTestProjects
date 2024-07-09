Feature: WebAction

Perform web action periodically

@tag1
Scenario: Perform download action from web periodically
	Given I have set up the web action
	When I start the periodic action
	Then the action should run every 10 minutes for 8 hours
	And I obtain file path of downloaded file from local directory

@tag1
Scenario Outline: Read downloaded PDF File
	#Given I have a PDF file "C:\Users\Abinaya\Downloads\magic.pdf"
	Given I have a PDF File
	When I read the file 
	Then I display contents of PDF file

