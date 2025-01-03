# C Package Manager

![Build](https://github.com/pieza/cpm/actions/workflows/release.yml/badge.svg)

> **Note:** This project is still under development, and the package repository is not yet functional.

## Description

CPM is a package manager designed for C developers transitioning from environments like Node.js. Its primary purpose is to provide an easy way to initialize a C project and reuse code efficiently.

## Installation

### Unix Systems

You can easily download and install **`cpm`** using the provided `install.sh` script:

```bash
curl -qL https://raw.githubusercontent.com/pieza/cpm/main/install.sh | bash
```
Then you need to update your shell settings to update the PATH (.bashrc, .bash_profile, etc)
```bash
export PATH="$PATH:$HOME/.cpm/bin"
```

This script installs `cpm` in the home directory at `~/.cpm/bin`.

Alternatively, you can manually install it by downloading the binaries from the release page.

### Windows
-- TODO

## Usage
-- TODO

## Uninstallation

### Linux

Regardless of whether you used the script, `cpm` will still create the `~/.cpm/` folder to store cache and settings. To uninstall, run the following command to delete the `cpm` home directory:

```bash
rm -rf ~/.cpm/
```

### Windows
-- TODO
