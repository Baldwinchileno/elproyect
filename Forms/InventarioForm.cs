﻿using System;
using System.Drawing;
using System.Windows.Forms;
using AdminSERMAC.Services;
using AdminSERMAC.Services.Database;
using Microsoft.Extensions.Logging;

namespace AdminSERMAC.Forms
{
    public class InventarioForm : Form
    {
        private readonly ILogger<SQLiteService> _logger;
        private readonly SQLiteService sqliteService;
        private readonly IInventarioDatabaseService _inventarioService;

        // Botones
        private Button comprarProductosButton;
        private Button visualizarInventarioButton;
        private Button crearProductoButton;
        private Button traspasoLocalButton;
        private Panel mainPanel;
        private Label titleLabel;

        public InventarioForm(ILogger<SQLiteService> logger, IInventarioDatabaseService inventarioService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            sqliteService = new SQLiteService(_logger);
            _inventarioService = inventarioService ?? throw new ArgumentNullException(nameof(inventarioService));

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Gestión de Inventario - SERMAC";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Panel principal
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Título
            titleLabel = new Label
            {
                Text = "Gestión de Inventario",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 20),
                ForeColor = Color.FromArgb(0, 122, 204)
            };

            // Botones
            comprarProductosButton = CreateMenuButton("Comprar Productos", 100);
            visualizarInventarioButton = CreateMenuButton("Visualizar Inventario", 170);
            crearProductoButton = CreateMenuButton("Crear Producto", 240);
            traspasoLocalButton = CreateMenuButton("Traspaso a Local", 310);

            // Configurar colores especiales para cada botón
            comprarProductosButton.BackColor = Color.FromArgb(0, 122, 204);      // Azul
            visualizarInventarioButton.BackColor = Color.FromArgb(40, 167, 69);  // Verde
            crearProductoButton.BackColor = Color.FromArgb(23, 162, 184);        // Cyan
            traspasoLocalButton.BackColor = Color.FromArgb(255, 193, 7);         // Amarillo
            traspasoLocalButton.ForeColor = Color.Black;  // Texto negro para el botón amarillo

            // Eventos
            comprarProductosButton.Click += ComprarProductosButton_Click;
            visualizarInventarioButton.Click += VisualizarInventarioButton_Click;
            crearProductoButton.Click += CrearProductoButton_Click;
            traspasoLocalButton.Click += TraspasoLocalButton_Click;

            // Agregar controles al panel
            mainPanel.Controls.AddRange(new Control[] {
                titleLabel,
                comprarProductosButton,
                visualizarInventarioButton,
                crearProductoButton,
                traspasoLocalButton
            });

            // Agregar panel al formulario
            this.Controls.Add(mainPanel);
        }

        private Button CreateMenuButton(string text, int top)
        {
            var button = new Button
            {
                Text = text,
                Top = top,
                Left = 50,
                Width = 250,
                Height = 45,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(button.BackColor);

            return button;
        }

        private void ComprarProductosButton_Click(object sender, EventArgs e)
        {
            try
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());
                var compraLogger = loggerFactory.CreateLogger<CompraInventarioForm>();
                var compraForm = new CompraInventarioForm(sqliteService, compraLogger);
                compraForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de compra: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(ex, "Error al abrir el formulario de compra");
            }
        }

        private void VisualizarInventarioButton_Click(object sender, EventArgs e)
        {
            try
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());
                var visualizarInventarioLogger = loggerFactory.CreateLogger<VisualizarInventarioForm>();
                var visualizarInventarioForm = new VisualizarInventarioForm(sqliteService, visualizarInventarioLogger);
                visualizarInventarioForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el visualizador de inventario: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(ex, "Error al abrir el visualizador de inventario");
            }
        }

        private void CrearProductoButton_Click(object sender, EventArgs e)
        {
            try
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());
                var crearProductoLogger = loggerFactory.CreateLogger<CrearProductoForm>();
                using (var crearProductoForm = new CrearProductoForm(crearProductoLogger))
                {
                    if (crearProductoForm.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Producto creado exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de crear producto: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(ex, "Error al abrir formulario de crear producto");
            }
        }

        private void TraspasoLocalButton_Click(object sender, EventArgs e)
        {
            try
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());
                var traspasoLogger = loggerFactory.CreateLogger<TraspasosForm>();
                var traspasoForm = new TraspasosForm(traspasoLogger, _inventarioService, sqliteService);

                if (traspasoForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Traspaso realizado exitosamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al abrir el formulario de traspasos");
                MessageBox.Show($"Error al abrir el formulario de traspasos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}