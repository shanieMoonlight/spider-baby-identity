# Job Definitions

## Overview
This directory contains all job definitions as abstract classes that inherit from `AMyIdJobHandler`. These jobs are implemented in the Infrastructure layer.

Each set of jobs will have its own starter file in the containing folder. These starter files are responsible for:
- Starting the jobs.
- Setting intervals and other configurations.

## Interfaces

### `IMyIdJobService`
- This interface is responsible for running any of the defined jobs.
- It is implemented in the Infrastructure layer.

## Job Starter

The `JobStarter` class is responsible for:
- Calling all the individual starter classes.
- Setting things in motion for the jobs to execute.
