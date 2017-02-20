Feature: MergesortAlgorithm
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@Mergesort
Scenario: Sort numbers using Mergesort
	Given that I have an array of random numbers from 0 to 99.
	When I run Mergesort,
	Then the array will run sequentially.

@Mergesort
Scenario: Sort numbers using Mergesort and TPL
	Given that I have an array of random numbers from 0 to 99.
	When I run Mergesort with the Task Parallel Library,
	Then the array will run sequentially.

@Mergesort
Scenario: Sort large numbers using Mergesort
	Given that I have an array of random numbers from 0 to 1000000.
	When I run Mergesort,
	Then the array will run sequentially.

@Mergesort
Scenario: Sort large numbers using Mergesort and TPL
	Given that I have an array of random numbers from 0 to 1000000.
	When I run Mergesort with the Task Parallel Library,
	Then the array will run sequentially.