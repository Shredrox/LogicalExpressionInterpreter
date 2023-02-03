<h1 align="center">Logical Expression Interpreter</h1>
<p align="center">
    An application that can create logical functions and solve them, create truth tables, find expressions from truth tables and visualize expression trees.
</p>
<p align="center">
  <img
    src="GDI/ProjectScreenshots/main.png"
    style="display: inline-block; margin: 0 auto; max-width: 300px">
</p>

## 🖥️ The Project
My coursework for Data Structures and Algorithms in university. 
The program is written in C# on the framework WPF.

# Features: 
Note: All input must be in the given format.
## ⚡ Adding and removing logical functions // DEFINE & REMOVE Commands
The DEFINE command creates and stores a logical function with the input expression for future evaluation.<br>
**Example: DEFINE func1(a,b): a && !b**

The REMOVE cammand removes the function with the input name.<br>
**Example: REMOVE func1**

## ⚡ Solving of boolean expressions // SOLVE Command
The SOLVE command allows the user to solve the defined functions with boolean values<br>
**Example: SOLVE func1(true,false)** <br>

## ⚡ Creating a Truth Table for a logic function // ALL Command
The ALL command creates a Truth Table for a chosen logic function's expression <br>
**Example: ALL func14** 
<p align="center">
  <img
    src="GDI/ProjectScreenshots/truthTable.png"
    style="display: inline-block; margin: 0 auto; max-width: 300px">
</p>

## ⚡ Finding an expression from a given Truth Table // FIND Command
The FIND command takes a given truth table and uses an **Evolutionary algorithm** to find an expression that fits the table/gives the same result. 
It also checks the already defined user functions for any matches. <br>
**Example: FIND (+ a path to a file with a truth table)**

## ⚡ Expression Tree Visualization // DISPLAY Command
The DISPLAY command displays the binary expression tree of an expression to the screen. <br>
**Example: DISPLAY Func6**

### Other Commands:
PRINTALL - prints all defined functions <br>
EXIT - another way to close the program
