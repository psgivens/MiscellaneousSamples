


[<TechTalk.SpecFlow.Binding>]
module FSharpLeftistHeapStepDefinition

open TechTalk.SpecFlow
open Microsoft.VisualStudio.TestTools.UnitTesting

open Example

// For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

[<Binding>]
type Example ()= 
    let mutable properties = []
    let mutable heap = HeapNode.Empty

    let [<Given>] ``I have a list with the following numbers``(values:Table) = 
        let rec insertItems i =
            if i < 0 then ()
            else 
                let value = int ((values.Rows.Item i).Item 0)            
                properties <- value::properties
                insertItems (i-1)
        insertItems (values.RowCount - 1)

    let [<When>] ``I enter the list into the Heap``() = 
        heap <- List.toHeap properties

    let [<Then>] ``the numbers come out in this order``(values:Table) = 
        let node = ref heap
        let rec assertItems i = 
            if i >= values.RowCount then ()
            else
                let value = int ((values.Rows.Item i).Item 0)
                match node.Value with
                | Empty -> Assert.Fail("Expecting another element at {0}", i)
                | Node(e,r,a,b) -> Assert.AreEqual(value, e)
                node.Value <- Heap.removeHead node.Value
                assertItems (i+1)
        assertItems 0
                



