import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal, viewChild, type OnInit } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import {
  Api,
  getLoadById,
  type DocumentType,
  type GeoPoint,
  type LoadDto,
  type LoadExceptionDto,
} from "@logistics/shared/api";
import { Grid, Icon, Stack, Surface, Typography } from "@logistics/shared/components";
import {
  AddressPipe,
  CurrencyFormatPipe,
  DateFormatPipe,
  DistanceUnitPipe,
} from "@logistics/shared/pipes";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { DividerModule } from "primeng/divider";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { TabsModule } from "primeng/tabs";
import { DirectionMap, DocumentManager, PageHeader, type Waypoint } from "@/shared/components";
import { LoadStatusTag, LoadTypeTag } from "@/shared/components/tags";
import {
  LoadExceptionsContent,
  LoadPodContent,
  LoadStatusStepper,
  ReportExceptionDialog,
  ResolveExceptionDialog,
  TrackingLinkDialog,
} from "../components";

@Component({
  selector: "app-load-detail",
  templateUrl: "./load-detail.html",
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    TabsModule,
    DividerModule,
    ProgressSpinnerModule,
    DateFormatPipe,
    CurrencyFormatPipe,
    PageHeader,
    LoadStatusTag,
    LoadTypeTag,
    AddressPipe,
    DistanceUnitPipe,
    DirectionMap,
    DocumentManager,
    LoadStatusStepper,
    LoadPodContent,
    LoadExceptionsContent,
    TrackingLinkDialog,
    ReportExceptionDialog,
    ResolveExceptionDialog,
    Grid,
    Icon,
    Stack,
    Surface,
    Typography,
  ],
})
export class LoadDetailPage implements OnInit {
  private readonly api = inject(Api);
  private readonly router = inject(Router);

  private readonly exceptionsContent = viewChild(LoadExceptionsContent);

  protected readonly id = input.required<string>();
  protected readonly isLoading = signal(false);
  protected readonly load = signal<LoadDto | null>(null);

  /** Origin + destination waypoints for the route map (numbered 1, 2). */
  protected readonly routeWaypoints = computed<Waypoint[]>(() => {
    const l = this.load();
    if (!l?.originLocation || !l?.destinationLocation) return [];
    return [
      { id: "origin", location: l.originLocation },
      { id: "destination", location: l.destinationLocation },
    ];
  });

  /** Truck/current location for the map — only while the load is not yet delivered. */
  protected readonly truckLocation = computed<GeoPoint | null>(() => {
    const l = this.load();
    if (!l || l.status === "delivered") return null;
    return l.currentLocation ?? null;
  });
  protected readonly activeTab = signal(0);
  protected readonly showTrackingDialog = signal(false);

  // Exception dialog state
  protected readonly showReportExceptionDialog = signal(false);
  protected readonly showResolveExceptionDialog = signal(false);
  protected readonly selectedExceptionToResolve = signal<LoadExceptionDto | null>(null);

  // Document types for the Documents tab
  protected readonly loadDocTypes: DocumentType[] = [
    "bill_of_lading",
    "proof_of_delivery",
    "invoice",
    "receipt",
    "contract",
    "photo",
    "other",
  ];

  ngOnInit(): void {
    this.fetchLoad();
  }

  onTabChange(index: string | number | undefined): void {
    if (typeof index !== "number") return;
    this.activeTab.set(index);
  }

  onEdit(): void {
    this.router.navigate(["/loads", this.id(), "edit"]);
  }

  onReportException(): void {
    this.showReportExceptionDialog.set(true);
  }

  onResolveException(exception: LoadExceptionDto): void {
    this.selectedExceptionToResolve.set(exception);
    this.showResolveExceptionDialog.set(true);
  }

  onExceptionReported(): void {
    this.exceptionsContent()?.refresh();
  }

  onExceptionResolved(): void {
    this.selectedExceptionToResolve.set(null);
    this.exceptionsContent()?.refresh();
  }

  private async fetchLoad(): Promise<void> {
    if (!this.id()) return;

    this.isLoading.set(true);
    const result = await this.api.invoke(getLoadById, { id: this.id() });
    if (result) {
      this.load.set(result);
    }
    this.isLoading.set(false);
  }
}
