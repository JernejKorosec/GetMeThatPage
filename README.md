# Get Me That Page

A project I did I had a few hours to spend :)


## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Introduction

The project is a web scrapper for a specific web page, Just run the code and your page gets scrapped.
Since it is a 3 to 6 hour project it was throughly tested only on a specific web page https://books.toscrape.com

It is not for anyone possible to finish a project like this in whole or as a sensible usable part in 3 hours.
Hence, it has only a part of functionality, not enough Iterations to find a resuable
blocks of code to create usable design patterns.

But otherwise, it builds without error, has seperated classes depending on functionality,
no comments and for such a short time given only asynchonously downloads files on depth 2.

Builds and tests were deployed and the scraping time was cut from 45 seconds
down to under 2 seconds. There is still refactoring needed also proper naming conventions.

If this was a task for 2,3 weeks including documentation, test and iterative design,
I would probably make it an opensource project for scraping and develop it by
all the development standards needed. Enjoy :)

If you need a complete solution thats scrapes the whole page in under 10 seconds,
let me know.


## Features and Futures

There are many great features in code from Design Patterns, Future Implementation Ideas to Programming Principles:
- Singleton (DP)
- Builder (DP)
- Chaining (DP)
- Solid as possible (PP)
- CLI (FII)
- NON-Blocking (FII)
- Async (FII)
- MultiThreaded Scraper (FII)
- Http 3.0 (FII)
- Many more...

## Getting Started

You need Latest updates of Visual Studio 2022 Since the code runs and it is written in .Net 7 (Core 7) since Net 8 isn't released yet.

### Prerequisites

There are no specifical dependencies you should worry about, since the are included with the project settings and should be downloaded automatically.
Latest Visual studio 2022 is Used and HTML Agiltiy Pack Framework. No Loggers, Schedulers or other frameworks are included.
Just download unzip open and run. A folder in executable binary folder is created with the first layer of page.
For async Tasks to work properly with multiple nested pages, a bit more work is needed.

### Installation

You clone, build and run. 

## Usage

This Demo does not include any argument passing to the Console application. Just run the exe file.

## Added functionality

If you need any additional functionality, get in touch.

## Next Version (Next changelog)

The idea for the next version is pretty simple.
It uses the non-Blocking idea using Queues and concurent Data
Structures for non blocking Saving of data in the following areas,
with the help of async methods and Tasks:
- Saving page(HTML) to Disk
- Saving resources to Disk
- Getting Data from Web

Anywhere where there is an option to have a concurent read or write,
to disable the possibility of deadlock, we use lock statement,
the function should be async and no Wait or Thread Sleep should be applied.

There is also a need for performance test on multiple web resources and pages.

- There also should be:
    - Test Driven Development (Only a few of the FWs)
        - NUnit or xUnit
        - Moq
        - SpecFlow
        - XUnitRunner
    - Proper branching (test, prod, dev, release)
    - Proper Logging using different Loggers (Logger facade):
        - Serilog
        - NLog
        - Log4Net

## Probable refactoring/bugfix needed for Multithreaded solution
- No static methos and object
- Rethink nullable and null checking and ifs
- Create function for multiple checking as a single call for prettier usage 

## Contact

You can reach me at :email:[lead.razvijalec@gmail.com](mailto:lead.razvijalec@gmail.com)

