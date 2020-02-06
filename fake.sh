#!/bin/bash

dotnet restore build.proj
dotnet fake $@
