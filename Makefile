TAG=$(shell echo NO_TAG_NAME_PROVIDED)
VERSION=$(shell git describe --tags --abbrev=0)-$(shell git rev-parse --short HEAD)
APP_NAME=$(shell basename $(shell git remote get-url origin) | tr 'A-Z' 'a-z' | cut -d '.' -f1 )


tag:
	git tag ${TAG}


run:
	dotnet run ImageOpt.fs;

build:
	dotnet build -c Release_${VERSION}