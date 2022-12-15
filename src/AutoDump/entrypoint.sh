#!/bin/bash
mkdir -p /var/docker/dumps/
procdump -e -w dotnet &
dotnet $1