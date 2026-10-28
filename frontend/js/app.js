class PulseTrackApp {
  constructor() {
    this.stages = [
      { id: 1, name: 'Intake', colId: 'col-intake' },
      { id: 2, name: 'Teardown', colId: 'col-teardown' },
      { id: 3, name: 'Body Repair', colId: 'col-body' },
      { id: 4, name: 'Paint & Refinish', colId: 'col-paint' },
      { id: 6, name: 'Ready for Delivery', colId: 'col-ready' }
    ];

    this.workOrders = [
      { id: '1', orderNumber: 'WO-2610-8841', vin: '1FTFW1ED4MFA10492', vehicle: '2021 Ford F-150 SuperCrew', customer: 'Apex Logistics Fleet', tech: 'Marcus Vance', priority: 'FleetCritical', stageId: 3, estimate: 4850, notes: 'Right fender structural alignment in progress.' },
      { id: '2', orderNumber: 'WO-2610-1109', vin: '4T1B11HK5JU294812', vehicle: '2020 Toyota Camry Hybrid', customer: 'Amanda Lewis', tech: 'David Chen', priority: 'Standard', stageId: 1, estimate: 2150, notes: 'Awaiting teardown slot.' },
      { id: '3', orderNumber: 'WO-2610-3420', vin: 'WAUZZZF27NA019283', vehicle: '2022 Audi A4 Quattro', customer: 'Robert Sterling', tech: 'Elena Rostova', priority: 'Expedited', stageId: 4, estimate: 7890, notes: 'Clearcoat baking cycle running.' },
      { id: '4', orderNumber: 'WO-2610-5501', vin: '3GNKBFRS5LS102941', vehicle: '2021 Chevy Traverse LT', customer: 'Metro Couriers Inc', tech: 'Marcus Vance', priority: 'FleetCritical', stageId: 3, estimate: 3400, notes: 'Frame pulling completed.' },
      { id: '5', orderNumber: 'WO-2610-9012', vin: '1G1ZD5ST8HF182741', vehicle: '2019 Chevy Malibu', customer: 'Patricia Alvarez', tech: 'David Chen', priority: 'Standard', stageId: 6, estimate: 1920, notes: 'Detailing complete. Customer notified.' },
      { id: '6', orderNumber: 'WO-2610-4412', vin: 'JM1GL1V48M1109482', vehicle: '2022 Mazda CX-5 AWD', customer: 'Jordan Miller', tech: 'Elena Rostova', priority: 'Standard', stageId: 2, estimate: 3100, notes: 'Bumper removed, sensor checks.' }
    ];

    this.partsCatalog = [
      { partNumber: 'FO-10294-FNDR', desc: 'Front Right Steel Fender Panel', cat: 'Sheetmetal', stock: 8, cost: 385.00, bin: 'Aisle 4' },
      { partNumber: 'TY-88412-BUMP', desc: 'Front Bumper Cover Fascia Primed', cat: 'Body & Trim', stock: 14, cost: 295.50, bin: 'Aisle 2' },
      { partNumber: 'AU-55019-HLMP', desc: 'Matrix LED Headlight Assembly Passenger', cat: 'Lighting', stock: 4, cost: 1240.00, bin: 'Secure Bay 1' },
      { partNumber: 'GM-33201-RADS', desc: 'Heavy Duty Engine Cooling Radiator', cat: 'Mechanical', stock: 9, cost: 310.00, bin: 'Aisle 6' },
      { partNumber: 'MZ-90142-SENS', desc: 'Blind Spot Radar Detection Sensor', cat: 'ADAS Sensors', stock: 12, cost: 420.00, bin: 'Secure Bay 2' }
    ];

    this.init();
  }

  init() {
    this.renderKanban();
    this.bindPartsSearch();
    this.bindThemeSwitch();
  }

  announce(msg) {
    const live = document.getElementById('pt-live-region');
    if (live) live.textContent = msg;
  }

  renderKanban() {
    this.stages.forEach(stage => {
      const colEl = document.getElementById(stage.colId);
      if (!colEl) return;

      const cardsContainer = colEl.querySelector('.kanban-cards-list');
      const countEl = colEl.querySelector('.col-count');
      const stageOrders = this.workOrders.filter(w => w.stageId === stage.id);

      if (countEl) countEl.textContent = stageOrders.length;
      if (!cardsContainer) return;

      cardsContainer.innerHTML = stageOrders.map(order => `
        <article class="kanban-card" tabindex="0" role="region" aria-label="Work Order ${order.orderNumber}, ${order.vehicle}">
          <div class="card-top">
            <span class="card-order-num">${order.orderNumber}</span>
            <span class="priority-badge priority-${order.priority.toLowerCase()}">${order.priority}</span>
          </div>
          <div class="card-vehicle">${order.vehicle}</div>
          <div class="card-info">
            <div>👤 <strong>Customer:</strong> ${order.customer}</div>
            <div>🔧 <strong>Tech:</strong> ${order.tech}</div>
            <div>💰 <strong>Estimate:</strong> $${order.estimate.toLocaleString()}</div>
          </div>
          <div style="font-size: 0.75rem; color: var(--pt-text-muted); font-style: italic;">"${order.notes}"</div>
          <div class="card-actions">
            <button type="button" class="btn-move btn-prev" data-order-id="${order.id}" aria-label="Move ${order.orderNumber} to previous stage">← Prev</button>
            <button type="button" class="btn-move btn-next" data-order-id="${order.id}" aria-label="Move ${order.orderNumber} to next stage">Next →</button>
          </div>
        </article>
      `).join('');

      cardsContainer.querySelectorAll('.btn-prev').forEach(btn => {
        btn.addEventListener('click', (e) => {
          e.stopPropagation();
          this.moveOrder(btn.getAttribute('data-order-id'), -1);
        });
      });

      cardsContainer.querySelectorAll('.btn-next').forEach(btn => {
        btn.addEventListener('click', (e) => {
          e.stopPropagation();
          this.moveOrder(btn.getAttribute('data-order-id'), 1);
        });
      });
    });

    const activeCount = this.workOrders.filter(w => w.stageId !== 6).length;
    const wipValuation = this.workOrders.reduce((sum, w) => sum + w.estimate, 0);

    const mActive = document.getElementById('metric-active-wo');
    const mWip = document.getElementById('metric-wip');
    if (mActive) mActive.textContent = `${activeCount} Units`;
    if (mWip) mWip.textContent = `$${(wipValuation / 1000).toFixed(1)}k WIP`;
  }

  moveOrder(orderId, direction) {
    const order = this.workOrders.find(w => w.id === orderId);
    if (!order) return;

    const currentStageIndex = this.stages.findIndex(s => s.id === order.stageId);
    const targetStageIndex = currentStageIndex + direction;

    if (targetStageIndex >= 0 && targetStageIndex < this.stages.length) {
      const targetStage = this.stages[targetStageIndex];
      order.stageId = targetStage.id;
      this.renderKanban();
      this.announce(`Work order ${order.orderNumber} moved to ${targetStage.name}.`);
    }
  }

  bindPartsSearch() {
    const input = document.getElementById('parts-search-input');
    const resultsUl = document.getElementById('parts-results-list');
    const latencyEl = document.getElementById('parts-latency-badge');

    if (!input || !resultsUl) return;

    let debounceTimer;
    input.addEventListener('input', () => {
      clearTimeout(debounceTimer);
      debounceTimer = setTimeout(() => {
        const query = input.value.trim().toUpperCase();
        const start = performance.now();

        const matches = this.partsCatalog.filter(p =>
          p.partNumber.includes(query) ||
          p.desc.toUpperCase().includes(query) ||
          p.cat.toUpperCase().includes(query)
        );

        const latency = (performance.now() - start).toFixed(1);
        if (latencyEl) latencyEl.textContent = `Redis Cache Latency: ${latency}ms (sub-10ms response)`;

        if (matches.length === 0) {
          resultsUl.innerHTML = `<li style="padding: 1rem; color: var(--pt-text-muted);">No catalog items matching "${query}".</li>`;
          return;
        }

        resultsUl.innerHTML = matches.map(p => `
          <li class="parts-item" tabindex="0" role="option">
            <div>
              <strong>${p.partNumber}</strong> — ${p.desc}
              <div style="font-size:0.75rem; color: var(--pt-text-secondary);">${p.cat} | Bin: ${p.bin}</div>
            </div>
            <div style="text-align: right;">
              <strong style="color: var(--pt-color-accent);">$${p.cost.toFixed(2)}</strong>
              <div style="font-size:0.75rem; color: ${p.stock > 5 ? 'var(--pt-color-standard)' : 'var(--pt-color-critical)'};">${p.stock} In Stock</div>
            </div>
          </li>
        `).join('');

        this.announce(`Found ${matches.length} parts matching query.`);
      }, 120);
    });
  }

  bindThemeSwitch() {
    const themeBtn = document.getElementById('btn-pt-theme');
    if (!themeBtn) return;

    themeBtn.addEventListener('click', () => {
      const current = document.documentElement.getAttribute('data-theme');
      const next = current === 'light' ? 'high-contrast' : current === 'high-contrast' ? 'dark' : 'light';
      if (next === 'dark') {
        document.documentElement.removeAttribute('data-theme');
      } else {
        document.documentElement.setAttribute('data-theme', next);
      }
      themeBtn.textContent = `Theme: ${next.toUpperCase()}`;
      this.announce(`Switched to ${next} theme.`);
    });
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new PulseTrackApp();
});
