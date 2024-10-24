# The Tour Guy ProductManagement POC

## Description

Aim of this POC is to test a way to let a res api solution to communicate with external providers via queue system

1. one of the core goal is to simply let the rest api to communicate with other sources with simple configuration
2. should be simple to produce new external sources and integrate inside the rest api solution
3. the soulution should be conteinerized so that could be scaled especially for external providers.

## Solution composition

1. TheTourGuy.ProductSearcherApi
   contains the rest api part
2. TheTourGuy.Models
   containes models used in this POC
3. TheTourGuy.Interfaces
   Contains interfaces used in this POC
4. TheTourGuy.DTO
   netstandar 2.1 project so that the DTO could be used also in legacy platform
5. TheTourGuy.BasicWorker
   Basic worker used in each external worker
6. TheBigGuyWorker
   One of the worker
7. SomeOtherGuyWorker
   One of the worker

## Design explanation

![](Design.drawio.svg)