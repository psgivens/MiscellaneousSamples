Feature: QuicksortAlgorithm
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@Quicksort
Scenario: Sort numbers using Quicksort
	Given that I have an array of random numbers from 0 to 99.
	When I run quicksort,
	Then the array will run sequentially.

@Quicksort
Scenario: Sort numbers using Quicksort and TPL
	Given that I have an array of random numbers from 0 to 99.
	When I run quicksort with the Task Parallel Library,
	Then the array will run sequentially.

@Quicksort
Scenario: Sort large numbers using Quicksort
	Given that I have an array of random numbers from 0 to 1000000.
	When I run quicksort,
	Then the array will run sequentially.

@Quicksort
Scenario: Sort large numbers using Quicksort and TPL
	Given that I have an array of random numbers from 0 to 1000000.
	When I run quicksort with the Task Parallel Library,
	Then the array will run sequentially.