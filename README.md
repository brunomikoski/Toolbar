# Toolbar

<p align="center">
    <a href="https://github.com/brunomikoski/Toolbar/blob/main/LICENSE">
		<img alt="GitHub license" src ="https://img.shields.io/github/license/brunomikoski/Toolbar" />
	</a>

</p> 
<p align="center">
    <a href="https://openupm.com/packages/com.brunomikoski.toolbar/">
        <img src="https://img.shields.io/npm/v/com.brunomikoski.toolbar?label=openupm&amp;registry_uri=https://package.openupm.com" />
    </a>

  <a href="https://github.com/brunomikoski/Toolbar/issues">
     <img alt="GitHub issues" src ="https://img.shields.io/github/issues/brunomikoski/Toolbar" />
  </a>

  <a href="https://github.com/brunomikoski/Toolbar/pulls">
   <img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr/brunomikoski/Toolbar" />
  </a>

  <img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/brunomikoski/Toolbar" />
</p>

<p align="center">
    	<a href="https://github.com/brunomikoski">
        	<img alt="GitHub followers" src="https://img.shields.io/github/followers/brunomikoski?style=social">
	</a>	
	<a href="https://twitter.com/brunomikoski">
		<img alt="Twitter Follow" src="https://img.shields.io/twitter/follow/brunomikoski?style=social">
	</a>
</p>

A set of useful tools on regular unity development

## Features
- History Selection
- Custom Favorites Selection
- Find assets by GUID.
- Automatically scene loading (Always start playmode from specified scene)
- Save Scenes / Project
- Timescale control
- Super easy to extend! 
- Customizable


## How to use?
- Open the Toolbar on <kbd>Tools/Toolbar/Open</kbd>
- To create your own custom actions just extend: `ButtonToolbarItemBase`  

## FAQ

## System Requirements
Unity 2018.4.0 or later versions


## How to install

<details>
<summary>Add from OpenUPM <em>| via scoped registry, recommended</em></summary>

This package is available on OpenUPM: https://openupm.com/packages/com.brunomikoski.animationsequencer

To add it the package to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
  ```
  Name: OpenUPM
  URL:  https://package.openupm.com/
  Scope(s): com.brunomikoski
  ```
- click <kbd>Save</kbd>
- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `com.brunomikoski.toolbar`
- click <kbd>Add</kbd>
</details>

<details>
<summary>Add from GitHub | <em>not recommended, no updates :( </em></summary>

You can also add it directly from GitHub on Unity 2019.4+. Note that you won't be able to receive updates through Package Manager this way, you'll have to update manually.

- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `https://github.com/brunomikoski/Toolbar.git`
- click <kbd>Add</kbd>
</details>


