Feature: FSharpLeftistHeap
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@Heap @FSharp
Scenario: Use heap to sort a series of numbers
	Given I have a list with the following numbers
	| number |
	| 56     |
	| 65     |
	| 18     |
	| 12     |
	| 17     |
	| 47     |
	| 02     |
	When I enter the list into the Heap
	Then the numbers come out in this order
	| number |
	| 02     |
	| 12     |
	| 17     |
	| 18     |
	| 47     |
	| 56     |
	| 65     |

