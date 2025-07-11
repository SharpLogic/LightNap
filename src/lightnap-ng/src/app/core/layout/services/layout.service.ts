import { Injectable, computed, effect, inject, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { APP_NAME, IdentityService, LayoutConfigDto, ProfileService } from "@core";
import { updatePreset, updateSurfacePalette } from "@primeng/themes";
import Aura from "@primeng/themes/aura";
import Lara from "@primeng/themes/lara";
import Nora from "@primeng/themes/nora";
import { Subject } from "rxjs";
import { ColorPalette } from "../models/color-palette";
import { LayoutState } from "../models/layout-state";

@Injectable({
  providedIn: "root",
})
export class LayoutService {
  #identityService = inject(IdentityService);
  #profileService = inject(ProfileService);
  appName = inject(APP_NAME);

  #config = this.#profileService.getDefaultStyleSettings();

  #state: LayoutState = {
    staticMenuDesktopInactive: false,
    overlayMenuActive: false,
    configSidebarVisible: false,
    staticMenuMobileActive: false,
    menuHoverActive: false,
  };

  layoutConfig = signal<LayoutConfigDto>(this.#config);
  layoutState = signal<LayoutState>(this.#state);
  #overlayOpen = new Subject<any>();
  overlayOpen$ = this.#overlayOpen.asObservable();
  theme = computed(() => (this.layoutConfig()?.darkTheme ? "light" : "dark"));
  isSidebarActive = computed(() => this.layoutState().overlayMenuActive || this.layoutState().staticMenuMobileActive);
  isDarkTheme = computed(() => this.layoutConfig().darkTheme);
  primaryPalette = computed(() => this.layoutConfig().primary);
  surfacePalette = computed(() => this.layoutConfig().surface);
  preset = computed(() => this.layoutConfig().preset);
  menuMode = computed(() => this.layoutConfig().menuMode);
  isOverlay = computed(() => this.layoutConfig().menuMode === "overlay");
  transitionComplete = signal(false);

  readonly menuModeOptions = [
    { label: "Static", value: "static" },
    { label: "Overlay", value: "overlay" },
  ];

  readonly presets = {
    Aura,
    Lara,
    Nora,
  } as const;

  readonly surfaces: ColorPalette[] = [
    {
      name: "slate",
      palette: {
        0: "#ffffff",
        50: "#f8fafc",
        100: "#f1f5f9",
        200: "#e2e8f0",
        300: "#cbd5e1",
        400: "#94a3b8",
        500: "#64748b",
        600: "#475569",
        700: "#334155",
        800: "#1e293b",
        900: "#0f172a",
        950: "#020617",
      },
    },
    {
      name: "gray",
      palette: {
        0: "#ffffff",
        50: "#f9fafb",
        100: "#f3f4f6",
        200: "#e5e7eb",
        300: "#d1d5db",
        400: "#9ca3af",
        500: "#6b7280",
        600: "#4b5563",
        700: "#374151",
        800: "#1f2937",
        900: "#111827",
        950: "#030712",
      },
    },
    {
      name: "zinc",
      palette: {
        0: "#ffffff",
        50: "#fafafa",
        100: "#f4f4f5",
        200: "#e4e4e7",
        300: "#d4d4d8",
        400: "#a1a1aa",
        500: "#71717a",
        600: "#52525b",
        700: "#3f3f46",
        800: "#27272a",
        900: "#18181b",
        950: "#09090b",
      },
    },
    {
      name: "neutral",
      palette: {
        0: "#ffffff",
        50: "#fafafa",
        100: "#f5f5f5",
        200: "#e5e5e5",
        300: "#d4d4d4",
        400: "#a3a3a3",
        500: "#737373",
        600: "#525252",
        700: "#404040",
        800: "#262626",
        900: "#171717",
        950: "#0a0a0a",
      },
    },
    {
      name: "stone",
      palette: {
        0: "#ffffff",
        50: "#fafaf9",
        100: "#f5f5f4",
        200: "#e7e5e4",
        300: "#d6d3d1",
        400: "#a8a29e",
        500: "#78716c",
        600: "#57534e",
        700: "#44403c",
        800: "#292524",
        900: "#1c1917",
        950: "#0c0a09",
      },
    },
    {
      name: "soho",
      palette: {
        0: "#ffffff",
        50: "#ececec",
        100: "#dedfdf",
        200: "#c4c4c6",
        300: "#adaeb0",
        400: "#97979b",
        500: "#7f8084",
        600: "#6a6b70",
        700: "#55565b",
        800: "#3f4046",
        900: "#2c2c34",
        950: "#16161d",
      },
    },
    {
      name: "viva",
      palette: {
        0: "#ffffff",
        50: "#f3f3f3",
        100: "#e7e7e8",
        200: "#cfd0d0",
        300: "#b7b8b9",
        400: "#9fa1a1",
        500: "#87898a",
        600: "#6e7173",
        700: "#565a5b",
        800: "#3e4244",
        900: "#262b2c",
        950: "#0e1315",
      },
    },
    {
      name: "ocean",
      palette: {
        0: "#ffffff",
        50: "#fbfcfc",
        100: "#F7F9F8",
        200: "#EFF3F2",
        300: "#DADEDD",
        400: "#B1B7B6",
        500: "#828787",
        600: "#5F7274",
        700: "#415B61",
        800: "#29444E",
        900: "#183240",
        950: "#0c1920",
      },
    },
  ];

  readonly colors = [
    "emerald",
    "green",
    "lime",
    "orange",
    "amber",
    "yellow",
    "teal",
    "cyan",
    "sky",
    "blue",
    "indigo",
    "violet",
    "purple",
    "fuchsia",
    "pink",
    "rose",
  ];

  primaryColors = computed<ColorPalette[]>(() => {
    const presetPalette = this.presets[this.preset() as keyof typeof this.presets].primitive;
    const palettes: ColorPalette[] = [{ name: "noir", palette: {} }];

    this.colors.forEach(color => {
      palettes.push({
        name: color,
        palette: presetPalette?.[color as keyof typeof presetPalette] as ColorPalette["palette"],
      });
    });

    return palettes;
  });

  #initialized = false;

  constructor() {
    effect(() => {
      const config = this.layoutConfig();
      if (config) {
        this.onConfigUpdate();
      }
    });

    effect(() => {
      const config = this.layoutConfig();

      if (!this.#initialized || !config) {
        this.#initialized = true;
        return;
      }

      this.#handleDarkModeTransition(config);
    });

    this.#identityService
      .watchLoggedIn$()
      .pipe(takeUntilDestroyed())
      .subscribe(loggedIn => {
        if (loggedIn) {
          this.#profileService.getSettings().subscribe(settings => {
            this.layoutConfig.update(state => ({ ...state, ...settings.style }));
          });
        } else {
          this.layoutConfig.update(state => ({ ...state, ...this.#profileService.getDefaultStyleSettings() }));
        }
      });
  }

  #handleDarkModeTransition(config: LayoutConfigDto): void {
    if ((document as any).startViewTransition) {
      this.#startViewTransition(config);
    } else {
      this.toggleDarkMode(config);
      this.#onTransitionEnd();
    }
  }

  #startViewTransition(config: LayoutConfigDto): void {
    const transition = (document as any).startViewTransition(() => {
      this.toggleDarkMode(config);
    });

    transition.ready.then(() => this.#onTransitionEnd()).catch(() => {});
  }

  toggleDarkMode(config?: LayoutConfigDto): void {
    const _config = config || this.layoutConfig();
    if (_config.darkTheme) {
      document.documentElement.classList.add("app-dark");
    } else {
      document.documentElement.classList.remove("app-dark");
    }
  }

  #onTransitionEnd() {
    this.transitionComplete.set(true);
    setTimeout(() => {
      this.transitionComplete.set(false);
    });
  }

  onMenuToggle() {
    if (this.isOverlay()) {
      this.layoutState.update(prev => ({ ...prev, overlayMenuActive: !this.layoutState().overlayMenuActive }));

      if (this.layoutState().overlayMenuActive) {
        this.#overlayOpen.next(null);
      }
    }

    if (this.isDesktop()) {
      this.layoutState.update(prev => ({ ...prev, staticMenuDesktopInactive: !this.layoutState().staticMenuDesktopInactive }));
    } else {
      this.layoutState.update(prev => ({ ...prev, staticMenuMobileActive: !this.layoutState().staticMenuMobileActive }));

      if (this.layoutState().staticMenuMobileActive) {
        this.#overlayOpen.next(null);
      }
    }
  }

  isDesktop() {
    return window.innerWidth > 991;
  }

  isMobile() {
    return !this.isDesktop();
  }

  onConfigUpdate() {
    const presetChanged = this.#config.preset !== this.layoutConfig().preset;
    if (presetChanged) {
      this.updatePreset();
    }

    if (presetChanged || this.#config.primary !== this.layoutConfig().primary) {
      this.updatePalette();
    }

    if (this.#config.surface !== this.layoutConfig().surface) {
      this.updateSurfacePalette();
    }

    this.#config = { ...this.layoutConfig() };

    if (this.#profileService.hasLoadedStyleSettings()) {
      this.#profileService.updateStyleSettings(this.#config).subscribe({
        error: response => console.error("Unable to save settings", response.errorMessages),
      });
    }
  }

  getPresetExt() {
    const color: ColorPalette = this.primaryColors().find(c => c.name === this.layoutConfig().primary) || {};

    const preset = this.layoutConfig().preset;

    if (color.name === "noir") {
      return {
        semantic: {
          primary: {
            50: "{surface.50}",
            100: "{surface.100}",
            200: "{surface.200}",
            300: "{surface.300}",
            400: "{surface.400}",
            500: "{surface.500}",
            600: "{surface.600}",
            700: "{surface.700}",
            800: "{surface.800}",
            900: "{surface.900}",
            950: "{surface.950}",
          },
          colorScheme: {
            light: {
              primary: {
                color: "{primary.950}",
                contrastColor: "#ffffff",
                hoverColor: "{primary.800}",
                activeColor: "{primary.700}",
              },
              highlight: {
                background: "{primary.950}",
                focusBackground: "{primary.700}",
                color: "#ffffff",
                focusColor: "#ffffff",
              },
            },
            dark: {
              primary: {
                color: "{primary.50}",
                contrastColor: "{primary.950}",
                hoverColor: "{primary.200}",
                activeColor: "{primary.300}",
              },
              highlight: {
                background: "{primary.50}",
                focusBackground: "{primary.300}",
                color: "{primary.950}",
                focusColor: "{primary.950}",
              },
            },
          },
        },
      };
    } else {
      if (preset === "Nora") {
        return {
          semantic: {
            primary: color.palette,
            colorScheme: {
              light: {
                primary: {
                  color: "{primary.600}",
                  contrastColor: "#ffffff",
                  hoverColor: "{primary.700}",
                  activeColor: "{primary.800}",
                },
                highlight: {
                  background: "{primary.600}",
                  focusBackground: "{primary.700}",
                  color: "#ffffff",
                  focusColor: "#ffffff",
                },
              },
              dark: {
                primary: {
                  color: "{primary.500}",
                  contrastColor: "{surface.900}",
                  hoverColor: "{primary.400}",
                  activeColor: "{primary.300}",
                },
                highlight: {
                  background: "{primary.500}",
                  focusBackground: "{primary.400}",
                  color: "{surface.900}",
                  focusColor: "{surface.900}",
                },
              },
            },
          },
        };
      } else {
        return {
          semantic: {
            primary: color.palette,
            colorScheme: {
              light: {
                primary: {
                  color: "{primary.500}",
                  contrastColor: "#ffffff",
                  hoverColor: "{primary.600}",
                  activeColor: "{primary.700}",
                },
                highlight: {
                  background: "{primary.50}",
                  focusBackground: "{primary.100}",
                  color: "{primary.700}",
                  focusColor: "{primary.800}",
                },
              },
              dark: {
                primary: {
                  color: "{primary.400}",
                  contrastColor: "{surface.900}",
                  hoverColor: "{primary.300}",
                  activeColor: "{primary.200}",
                },
                highlight: {
                  background: "color-mix(in srgb, {primary.400}, transparent 84%)",
                  focusBackground: "color-mix(in srgb, {primary.400}, transparent 76%)",
                  color: "rgba(255,255,255,.87)",
                  focusColor: "rgba(255,255,255,.87)",
                },
              },
            },
          },
        };
      }
    }
  }

  updatePalette() {
    updatePreset(this.getPresetExt());
  }

  updateSurfacePalette() {
    updateSurfacePalette(this.surfaces.find(s => s.name === this.layoutConfig().surface)?.palette);
  }

  updatePreset() {
    updatePreset(this.presets[this.layoutConfig().preset as keyof typeof this.presets]);
  }
}
