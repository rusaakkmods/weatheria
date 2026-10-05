const countrySelect = document.querySelector("#country-select");
const countryStatus = document.querySelector("#country-status");
const citySelect = document.querySelector("#city-select");
const cityStatus = document.querySelector("#city-status");
const weatherPanel = document.querySelector("#weather-panel");
const weatherStatus = document.querySelector("#weather-status");
const weatherMetrics = document.querySelector("#weather-metrics");
const weatherNoteForm = document.querySelector("#weather-note-form");
const weatherNoteInput = document.querySelector("#weather-note");
const saveNoteButton = document.querySelector("#save-note");
const noteStatus = document.querySelector("#note-status");
let cityRequestVersion = 0;
let weatherRequestVersion = 0;
let noteRequestVersion = 0;

function resetWeather() {
    weatherRequestVersion += 1;
    noteRequestVersion += 1;
    weatherPanel.hidden = true;
    weatherMetrics.hidden = true;
    weatherStatus.textContent = "";
    weatherNoteForm.hidden = true;
    weatherNoteForm.reset();
    saveNoteButton.disabled = true;
    noteStatus.textContent = "";
}

async function loadCountries() {
    countryStatus.textContent = "Loading countries...";

    try {
        const response = await fetch("/api/countries", {
            headers: { Accept: "application/json" }
        });

        if (!response.ok) {
            throw new Error(`Country request failed with status ${response.status}.`);
        }

        const countries = await response.json();
        if (!Array.isArray(countries)) {
            throw new Error("Country response was not a list.");
        }

        const countryOptions = countries
            .filter(country =>
                country &&
                typeof country.name === "string" &&
                typeof country.code === "string")
            .map(country => new Option(country.name, country.code));

        if (countries.length > 0 && countryOptions.length === 0) {
            throw new Error("Country response did not contain valid entries.");
        }

        countrySelect.replaceChildren(
            new Option("Select a country", ""),
            ...countryOptions);

        if (countryOptions.length === 0) {
            countryStatus.textContent = "No countries are currently available.";
            return;
        }

        countrySelect.disabled = false;
        countryStatus.textContent = `${countryOptions.length} countries available.`;
    } catch {
        countrySelect.replaceChildren(new Option("Countries unavailable", ""));
        countryStatus.textContent = "Unable to load countries. Refresh the page to try again.";
    }
}

async function loadCities(countryCode) {
    const requestVersion = ++cityRequestVersion;
    citySelect.disabled = true;
    citySelect.replaceChildren(new Option("Loading cities...", ""));
    cityStatus.textContent = "Loading cities...";

    try {
        const response = await fetch(
            `/api/countries/${encodeURIComponent(countryCode)}/cities`,
            { headers: { Accept: "application/json" } });

        if (!response.ok) {
            throw new Error(`City request failed with status ${response.status}.`);
        }

        const cities = await response.json();
        if (!Array.isArray(cities)) {
            throw new Error("City response was not a list.");
        }

        const cityOptions = cities
            .filter(city => city && typeof city.name === "string")
            .map(city => new Option(city.name, city.name));

        if (cities.length > 0 && cityOptions.length === 0) {
            throw new Error("City response did not contain valid entries.");
        }

        if (requestVersion !== cityRequestVersion) {
            return;
        }

        citySelect.replaceChildren(
            new Option(cityOptions.length > 0 ? "Select a city" : "No cities available", ""),
            ...cityOptions);

        if (cityOptions.length === 0) {
            cityStatus.textContent = "No cities are currently available for this country.";
            return;
        }

        citySelect.disabled = false;
        cityStatus.textContent = `${cityOptions.length} cities available.`;
    } catch {
        if (requestVersion !== cityRequestVersion) {
            return;
        }

        citySelect.replaceChildren(new Option("Cities unavailable", ""));
        cityStatus.textContent = "Unable to load cities. Change countries to try again.";
    }
}

async function loadWeather(cityName) {
    const requestVersion = ++weatherRequestVersion;
    weatherPanel.hidden = false;
    weatherMetrics.hidden = true;
    weatherStatus.textContent = `Loading weather for ${cityName}...`;

    try {
        const response = await fetch(
            `/api/weather/${encodeURIComponent(cityName)}`,
            { headers: { Accept: "application/json" } });

        if (!response.ok) {
            throw new Error(`Weather request failed with status ${response.status}.`);
        }

        const weather = await response.json();
        const observationTime = new Date(weather.timeUtc);
        if (!Number.isFinite(observationTime.getTime())) {
            throw new Error("Weather response contained an invalid UTC time.");
        }

        if (requestVersion !== weatherRequestVersion) {
            return;
        }

        document.querySelector("#weather-location").textContent =
            `${weather.city}, ${weather.country}`;
        const timeElement = document.querySelector("#weather-time");
        timeElement.dateTime = observationTime.toISOString();
        timeElement.textContent = `${new Intl.DateTimeFormat(undefined, {
            dateStyle: "medium",
            timeStyle: "short",
            timeZone: "UTC"
        }).format(observationTime)} UTC`;
        document.querySelector("#weather-temperature-f").textContent =
            `${weather.temperatureFahrenheit} °F`;
        document.querySelector("#weather-temperature-c").textContent =
            `${weather.temperatureCelsius} °C`;
        document.querySelector("#weather-dew-point").textContent =
            `${weather.dewPointFahrenheit} °F`;
        document.querySelector("#weather-humidity").textContent =
            `${weather.relativeHumidityPercent}%`;
        document.querySelector("#weather-wind-speed").textContent =
            `${weather.windSpeedMph} mph`;
        document.querySelector("#weather-wind-direction").textContent =
            `${weather.windDirectionDegrees}°`;
        document.querySelector("#weather-visibility").textContent =
            `${weather.visibilityMeters} m`;
        document.querySelector("#weather-pressure").textContent =
            `${weather.pressureHpa} hPa`;
        document.querySelector("#weather-sky-condition").textContent = weather.skyCondition;

        weatherMetrics.hidden = false;
        weatherStatus.textContent = "Current weather loaded.";
        weatherNoteForm.hidden = false;
        saveNoteButton.disabled = false;
    } catch {
        if (requestVersion !== weatherRequestVersion) {
            return;
        }

        weatherStatus.textContent = "Unable to load weather. Select the city again to retry.";
    }
}

if (weatherNoteForm) {
    weatherNoteForm.addEventListener("submit", async event => {
        event.preventDefault();

        const cityName = citySelect.value;
        const content = weatherNoteInput.value.trim();
        if (!cityName || !content) {
            noteStatus.textContent = "Enter a note before saving.";
            return;
        }

        const requestVersion = ++noteRequestVersion;
        saveNoteButton.disabled = true;
        noteStatus.textContent = "Saving note...";

        try {
            const response = await fetch("/api/weather/notes", {
                method: "POST",
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ city: cityName, content })
            });

            if (!response.ok) {
                throw new Error(`Note request failed with status ${response.status}.`);
            }

            const result = await response.json();
            if (typeof result.id !== "string" || !result.id) {
                throw new Error("Note response did not contain an identifier.");
            }

            if (requestVersion !== noteRequestVersion || cityName !== citySelect.value) {
                return;
            }

            if (weatherNoteInput.value.trim() === content) {
                weatherNoteInput.value = "";
            }
            noteStatus.textContent = "Weather note saved.";
        } catch {
            if (requestVersion === noteRequestVersion && cityName === citySelect.value) {
                noteStatus.textContent = "Unable to save note. Please try again.";
            }
        } finally {
            if (requestVersion === noteRequestVersion && cityName === citySelect.value) {
                saveNoteButton.disabled = false;
            }
        }
    });
}

if (countrySelect && citySelect && cityStatus) {
    countrySelect.addEventListener("change", () => {
        resetWeather();
        cityRequestVersion += 1;
        citySelect.disabled = true;
        citySelect.replaceChildren(new Option("Select a country first", ""));
        cityStatus.textContent = "Select a country first.";

        if (countrySelect.value) {
            loadCities(countrySelect.value);
        }
    });
}

if (citySelect) {
    citySelect.addEventListener("change", () => {
        resetWeather();

        if (citySelect.value) {
            loadWeather(citySelect.value);
        }
    });
}

if (countrySelect && countryStatus) {
    loadCountries();
}