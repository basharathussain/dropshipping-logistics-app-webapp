import { Component, inject, signal, type OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import {
  Api,
  getAiSettings,
  testAiKey,
  updateAiSettings,
  type PlanQuotaDto,
} from "@logistics/shared/api";
import { Grid, PageHeader, Stack, Typography } from "@logistics/shared/components";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { CheckboxModule } from "primeng/checkbox";
import { InputNumberModule } from "primeng/inputnumber";
import { InputTextModule } from "primeng/inputtext";
import { MessageModule } from "primeng/message";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { SelectModule } from "primeng/select";
import { ToastService } from "@/core/services";
import { TenantQuotas } from "../tenant-quotas/tenant-quotas";

interface ModelOption {
  label: string;
  value: string;
}

type KeyStatus = "unknown" | "valid" | "invalid";

@Component({
  selector: "adm-ai-settings",
  templateUrl: "./ai-settings.html",
  imports: [
    FormsModule,
    ButtonModule,
    CardModule,
    CheckboxModule,
    InputNumberModule,
    InputTextModule,
    MessageModule,
    ProgressSpinnerModule,
    SelectModule,
    Grid,
    PageHeader,
    Stack,
    Typography,
    TenantQuotas,
  ],
})
export class AiSettings implements OnInit {
  private readonly api = inject(Api);
  private readonly toastService = inject(ToastService);

  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);

  protected readonly selectedModel = signal("");
  protected readonly extendedThinking = signal(false);
  protected readonly modelOptions = signal<ModelOption[]>([]);
  protected readonly plans = signal<PlanQuotaDto[]>([]);

  // API key (per the selected model's provider). The input stays empty unless the admin types a new
  // key; a saved key is shown only as a masked placeholder. The Test button validates with a live call.
  protected readonly apiKey = signal("");
  protected readonly apiKeyMasked = signal<string | null>(null);
  protected readonly hasApiKey = signal(false);
  protected readonly isTestingKey = signal(false);
  protected readonly keyStatus = signal<KeyStatus>("unknown");
  protected readonly keyStatusMessage = signal("");

  ngOnInit(): void {
    this.load();
  }

  private async load(): Promise<void> {
    this.isLoading.set(true);
    try {
      const settings = await this.api.invoke(getAiSettings);
      this.selectedModel.set(settings.model ?? "");
      this.extendedThinking.set(settings.extendedThinking ?? false);
      this.modelOptions.set(
        (settings.availableModels ?? []).map((m) => ({
          label: m.displayName ?? m.id ?? "",
          value: m.id ?? "",
        })),
      );
      this.plans.set(settings.plans ?? []);
      this.hasApiKey.set(settings.hasApiKey ?? false);
      this.apiKeyMasked.set(settings.apiKeyMasked ?? null);
      this.apiKey.set("");
      this.keyStatus.set("unknown");
      this.keyStatusMessage.set("");
    } catch {
      this.toastService.showError("Failed to load AI settings");
    } finally {
      this.isLoading.set(false);
    }
  }

  protected async testKey(): Promise<void> {
    this.isTestingKey.set(true);
    this.keyStatus.set("unknown");
    this.keyStatusMessage.set("");
    try {
      const result = await this.api.invoke(testAiKey, {
        body: {
          model: this.selectedModel(),
          apiKey: this.apiKey().trim() || undefined,
        },
      });
      const valid = result.valid ?? false;
      this.keyStatus.set(valid ? "valid" : "invalid");
      this.keyStatusMessage.set(
        result.message ?? (valid ? "Connection successful." : "Invalid key."),
      );
    } catch {
      this.keyStatus.set("invalid");
      this.keyStatusMessage.set("Test request failed.");
    } finally {
      this.isTestingKey.set(false);
    }
  }

  protected updatePlanQuota(planId: string | undefined, quota: number | null): void {
    this.plans.update((plans) =>
      plans.map((p) => (p.planId === planId ? { ...p, weeklyAiRequestQuota: quota } : p)),
    );
  }

  protected async save(): Promise<void> {
    this.isSaving.set(true);
    try {
      await this.api.invoke(updateAiSettings, {
        body: {
          model: this.selectedModel(),
          extendedThinking: this.extendedThinking(),
          // Only send a key when the admin typed one; blank leaves the saved key unchanged.
          apiKey: this.apiKey().trim() || undefined,
          plans: this.plans().map((p) => ({
            planId: p.planId,
            weeklyAiRequestQuota: p.weeklyAiRequestQuota,
          })),
        },
      });
      this.toastService.showSuccess("AI settings saved successfully");
      await this.load();
    } catch {
      this.toastService.showError("Failed to save AI settings");
    } finally {
      this.isSaving.set(false);
    }
  }
}
