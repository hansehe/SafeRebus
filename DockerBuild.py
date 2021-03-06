import subprocess
import sys
import os
import yaml
from DockerBuildSystem import TerminalTools, DockerComposeTools, VersionTools, DockerSwarmTools
from SwarmManagement import SwarmManager

AvailableCommands = [
    ['run', 'Run solution.'],
    ['build', 'Build solution.'],
    ['test', 'Test with integration testing.'],
    ['publish', 'Publish nuget packages.'],
    ['start-dev', 'Start development session by deploying service dependencies'],
    ['stop-dev', 'Stop development session by removing service dependencies'],
    ['help', 'Print available argument commands.']
]

def BuildDocker(buildSelection):
    VersionTools.ExportVersionFromChangelogToEnvironment('CHANGELOG.md', 'VERSION')
    srcFolder = ['src', '..']
    miniSwarmManagementFile = 'swarm-management.yml'
    migratorComposeFiles = [
        'docker-compose.migrator.yml'
    ]
    nugetDockerComposeFiles = [
        'docker-compose.publish.nuget.yml'
    ]
    generalComposeFiles = [
        'docker-compose.message.handler.yml',
        'docker-compose.outboxcleaner.yml',
        'docker-compose.message.spammer.yml'
    ]

    if buildSelection == 'run':
        os.chdir(srcFolder[0])
        DockerComposeTools.DockerComposeUp(migratorComposeFiles)
        DockerComposeTools.DockerComposeUp(generalComposeFiles)
        os.chdir(srcFolder[1])
    
    elif buildSelection == 'build':
        os.chdir(srcFolder[0])
        DockerComposeTools.DockerComposeBuild(generalComposeFiles + migratorComposeFiles)
        os.chdir(srcFolder[1])

    elif buildSelection == 'test':
        os.chdir(srcFolder[0])
        DockerComposeTools.ExecuteComposeTests(
            ['docker-compose.tests.yml'], ['saferebus-tests'])
        os.chdir(srcFolder[1])

    elif buildSelection == 'publish':
        os.chdir(srcFolder[0])
        DockerComposeTools.DockerComposeBuild(nugetDockerComposeFiles)
        DockerComposeTools.DockerComposeUp(nugetDockerComposeFiles, False)
        os.chdir(srcFolder[1])

    elif buildSelection == 'start-dev':
        os.chdir(srcFolder[0] + '/ServiceDependencies')
        SwarmManager.HandleManagement(['-start', '-f', miniSwarmManagementFile])
        os.chdir(srcFolder[1] + '/..')

    elif buildSelection == 'stop-dev':
        os.chdir(srcFolder[0] + '/ServiceDependencies')
        SwarmManager.HandleManagement(['-stop', '-f', miniSwarmManagementFile])
        os.chdir(srcFolder[1] + '/..')

    elif buildSelection == 'help':
        TerminalTools.PrintAvailableCommands(AvailableCommands)

    else:
        print('Please provide a valid build argument: ')
        BuildDocker('help')

if __name__ == '__main__':
    buildSelections = sys.argv[1:]
    if len(buildSelections) == 0:
        buildSelections = ['no_argument']
    for buildSelection in buildSelections:
        BuildDocker(buildSelection)
