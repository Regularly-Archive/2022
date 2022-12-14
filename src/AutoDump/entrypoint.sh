#!/bin/bash
mkdir /usr/docker/dumps/
procdump -M 200 -w dotnet &
dotnet $1