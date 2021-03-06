# RaceAdmin
A tool for tracking iRacing incidents and recommending/throwing full course cautions.

## Quick Hints
* Any of the numeric input fields can be set to zero to completely disable the related functionality.
* You must be a session admin in order for this tool to function as intended since incidents for all
cars are only reported at the end of a race to non-admins.

## Overview

![race admin screenshot](/images/race-admin.png)

### Session/Caution Indicator Panel
This panel displays the name of the current session and will flash yellow when the caution conditions
have been met. Once the caution flag has been thrown in will remain solid yellow until the race resumes
under green flag conditions.

### Incident Table
Displays any incidents reported by iRacing.
* Time - the time the incident occurred referenced from the beginning of the current session
* Car # - the number of the car incurring the incident
* Team - (team races only) the name of the team incurring the incident
* Driver - the name of the driver incurring the incident
* Inc. - the type of incident incurred: 1x, 2x, 4x generally (see below)
* Total - the total number of incidents incurred by the car/driver (more below)
* Car Lap # - the lap the car was on when the incident occurred

Note that due to the way incidents are reported by iRacing, incidents promoted from a 1x to
a 2x or 4x may be reported as two separate 1x incidents or a 1x and a 3x. This may be addressed
in a future update.

During team racing, the Total column will display the team's total incidents followed by the 
driver's total incidents in the same fashion as iRacing does. For example if the team had a total
of 4 incidents and the driver had contributed two incidents at the time of the incident, this 
column would contain "4,2".

### Export
Allows the incidents to be exported to a CSV file.

### Total Incidents
Tracks the total number of incidents reported during the current session.

### Since Last Caution
Tracks the number of incidents reported since the last full course caution.

### Full Course Cautions
Options for controlling how the app determines when a full course caution should be thrown and
how it reacts in this case.

* Incidents required for caution - number of incidents after which a full course caution will be recommended; set to zero to totally disable full course cautions
* Use audio notification - play an alert sound when full course caution conditions are met
* Throw caution flag automatically (beta) - attempts to throw a full course caution by sending keypresses to iRacing using the Windows SendKeys functionality when full course caution conditions are met
* No cautions during last laps - don't recommend or throw cautions during the last number of laps specified; set to zero to allow cautions up until the checkered flag
* No cautions during last minutes - don't recommend or throw cautions during the last number of minutes specified; set to zero to allow cautions up until the checkered flag

The app can only honor the last laps and last minutes settings when iRacing is reporting laps remaining and/or time remaining via telemetry. If a race is configured as a race to a set number of laps, then the time remaining is not reported by iRacing unless iRacing detects that the session time will expire before the race completes. Similarly when a race is configured as a timed race only, iRacing does not report the number of laps remaining.

### General
General settings which don't fall into other categories.

* Hide incidents during race - obscures the incidents table, total incidents and incidents since last caution data during a race so that it will not distract a driver who is also using the app to assist with cautions.


## Command Line Options

While RaceAdmin normally runs as a simple form and processes live session and telemetry updates from iRacing, it also offers the ability to record and playback session data. This is a recent addition and is not fully mature. It's intended purpose is to allow users to record the session/telemetry data so that it can be used to debug program issues or to aid in new feature development. 

Available command line options:
```
RaceAdmin [options] [command]

Options:
  --version         Show version information
  -?, -h, --help    Show help and usage information

Commands:
  record                Record session updates and telemetry.
  playback <logfile>    Play back only events from the indexed session within the session log.
```
The output session log file is written into the current user's documents folder. The session log filename when recording will be `race-admin-session-<iRacingSessionID>.bin`. When using the playback command, the full path to the session log should not be specified, only the filename itself.
  
### Developer Notes
First, only telemetry fields currently used in this codebase are recorded. While this is mainly to keep the output file size to a minimum, it is also a consequence of the libraries used to read the iRacing telemetry updates. The exact fields captured are those exposed as properties by `ITelemetryInfo`. The initial set of fields captured are: `SessionFlags`, `SessionLapsRemain`, `SessionNum`, `SessionTimeRemain` and `SessionUniqueID`. A second difference is that the playback speed is much higher than the capture data rate. 

Additionally there are several interfaces and fakes introduced to facilitate unit test construction. While this goal is achieved, it does add more boilerplate and complication to the normal use case code. This layer may be encapsulated outside of the RaceAdmin codebase itself in the future.
