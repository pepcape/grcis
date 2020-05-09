# newmodules

This is directory for new student modules (in the current semester).

Special branch is created for every semester and lecturer[s] will
put modules here for public use/review. After a semester ends, the branch
will be purged (but valuable results will be archived somewhere).

## Winter semester

Modules for the `117raster` raster-image framework.

## Summer semester

Extensions which can be used in `048rtmontecarlo-script` and `062animation-script`
projects (Ray-tracing).

Please create one directory for each extension.
Name it
```
FirstnameSurname-ModuleName
```
The `ModuleName` is arbitrary name, please don't use whitespaces in directoery and file names.

Each new module should contain documentation in the `README.md`.

---

# GitHub instructions

You need to "Fork" our official GitHub repository `https://github.com/pepcape/grcis`. Please go to
[this](https://github.com/pepcape/grcis) page and select the command **"Fork"**. Set your repository as private but share it with me **https://github.com/pepcape**.

Switch to the **summer2019-2020** branch because in that branch new extensions of your colleagues will eventually appear (they won't be merged into `master` branch).

Check time to time new updates from the GitHub using the
```
git pull upstream master
```
(this needs checking on the actual "Forked" GitHub repository)

I'm going to propagate all new changes/updates both to the 'master' and the `summer2019-2020` branches.

# GitLab instructions

If you want to work on faculty's [GitLab](https://gitlab.mff.cuni.cz/), you have to connect your new private GitLab repo with the original GitHub GrCis in the way similar to the "Fork" command.

The recommended procedure is (see [this page](https://stackoverflow.com/questions/50973048/forking-git-repository-from-github-to-gitlab) for more information):

1. Create an **empty GitLab repository** (even without README.md) on faculty's server  `https://gitlab.mff.cuni.cz/`. Set access mode to **Private** -- you don't want your work to be visible publicly yet.
 I will use **<loc>** mark for your local/home computer, **<hub>** for our original GrCis repository and **<lab>** for your new private repository on faculty's GitLab. So the item 1. should be marked **<lab>**

2. **Clone it** to your home computer **<loc>**
   ```
   git clone <GitLab-repo-URL> <local-directory>
   ```
   or using **TortoiseGIT**
    1. local Windows menu -> `Git Clone..`
    2. Choose (check) the correct `URL` and enter your preferred directory name
    3. press the `OK` button

    **Remark:** I realized that I had to use the "SSH format" of the GitLab repo URL    because I wanted to use my SSH key to acces it (opposed to https access which needs name/password authentication via university's LDAP)

     The URL is
     `git@gitlab.mff.cuni.cz:<your-account-name>/<your-repo-name>.git`

3. **<loc>** Add the GitHub GrCis project to the **"upstream" remote** on your local computer (this is actually an equivalent to the "Fork" command) with
   ```
   git remote add upstream https://github.com/user/repo
   ```
   Perhaps this can be done using TortoiseGIT as well but I don't know how, sorry..

4. **<loc>** Now you can **checkout/switch** the upstream master branch and pull it from upstream
   ```
   git pull upstream master
   ```
   or in **TortoiseGIT**
    1.  use `Git Sync` (local menu command in File-manager)
    2.  choose the correct `Remote URL` (original GitHub - **upstream**)
    3.  run two commands: **Fetch all references** (just to be sure) and then **Pull**

5. **<loc>** Then you should **push (from home)** to your GitLab repository using
   ```
   git push origin master
   ```
   or in **TortoiseGIT**
    1.  use `Git Sync` (local menu command)
    2.  choose the correct `Remote URL` (your new GitLab - **origin**)
    3.  run one command: **Push**

6. **<loc>** From now on, you can work freely at your **hone clone** of your private GitLab project, **Commit** and **Push** to the GitLab

7. **<loc>** Time to time you should check the original GitHub repo and pull updates from there
   ```
   git pull upstream master
   ```
   Or use the **Pull** command in **TortoiseGIT** with proper settings (`Remote URL = upstream`)

---

## Contact

Please address all requests, bug reports and suggestions to
**Josef Pelikan <pepca@cgg.mff.cuni.cz>**
https://cgg.mff.cuni.cz/~pepca/

### Copyright notice

All the code in this repository (unless otherwise stated) is copyrighted
by [Josef Pelikan](https://cgg.mff.cuni.cz/~pepca/).

Charles University students retain the copyright to their work published here.
