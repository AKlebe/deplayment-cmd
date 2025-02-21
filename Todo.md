# TODO

## whole project

- replace new data version in release projects
  ````
  "___#inherit#___": "(.*?)",
  ````
  to
  ````
  "\$": {"require": [{"file": "$1"}]},
  ````

- project include multiple projects
- add system vars like application path, project path, etc
- PathAbsolute setter wrong behavior because not touching underlying root
- remove COPY_FILES_MULTI_PATH and implement it in copyFiles?
- finish base toBaseObject()
- normalize in metapath or deeper
- Git test branches etc after redesign
- adjust Project/Source/Git.cs deployments like path with multiple names
- Design to use multiple deployment containers
- ftp check
- git check
- zip check
- prepare more projects
  + sync music
  - sync music documents
  - sync shared media
  - sync ak-docs
- finish documentations / translate documentations
- Tests
- public for github
- optional backups (by optional backup path)

+ resolve warnings
+ resolve inherits for DeploymentCopyFilesMultiPath : DeploymentCopyFiles
+ system file copy doesnt changes file time, make same single point for copy function
+ console list all projects (by "-projects" ?)
+ console list all variants (by "-variants" ?)
+ Merge main loops for RunDirectories() and DirectoryCopy()
+ found created directory xxx#$CHARACTER_DIR$#
+ create regex lists for source, dest and all together
+ is it possible to use basic regex whitelists for multi copy regex?
+ Source currently for GIT only, make it valid for native sources
+ project debug and simulate (for anything) by cli option 
+ make placeholders case insensitivity
+ testing temp folder again
+ Add JsonIgnore where XmlIgnore to properties
+ find drive (by key-file?)
+ Design BaseObjectList
+ var include should not root from project source but from loaded json itself
+ smart place for normalize (immediate after parsing properties)
+ Find a place to parse all json values (in run()?), dont use xxxParsed properties
+ evaluate var not before they needed (drive ...) (use DeploymentVariablesItem.PreParse)
+ cleanup all FromBaseObject(from)


## Project Refactorings
- deployment_name to run_deployments
- json "variables" refactor childs code to name
- find and replace: "___#inherit#___": "(.*?)",
  to: "\$": {"require": [{"file": "$1"}]},
- refactor 
  - "copy_directory_settings" to "options"
  - "source_account" to "source_env"
  - "destination_account" to "destination_env"
  - source type "PATH" to "LOCAL"